using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TodoApp.Presentation.BackgroundJobs;
using TodoApp.Presentation.Identity;
using TodoApp.Presentation.Persistence;
using TodoApp.Presentation.Repositories;
using TodoApp.Presentation.Service;
using ToDoApp.Application.Concrete.Factories;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.BackgroundJob;
using ToDoApp.Application.Interfaces.BackgroundJobs;
using ToDoApp.Application.Interfaces.Factory;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Application.Services;
using ToDoApp.Domain.Entity;

namespace TodoApp.Presentation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // ============================
            // Controllers
            // ============================
            builder.Services.AddControllers();
            builder.Logging.AddConsole();


            // ============================
            // Database
            // ============================
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("MyConnection")
                );
            });

            builder.Services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();


            builder.Services.AddHangfire(configuration => configuration.UseSqlServerStorage(
                builder.Configuration.GetConnectionString("MyConnection")
            ));

            builder.Services.AddHangfireServer();




            // ============================
            // Dependency Injection
            // ============================
            builder.Services.AddScoped(
                typeof(IGenericRepository<,>),
                typeof(GenericRepository<,>)
            );
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUser , CurrentUser>();
            builder.Services.AddScoped<IAuthenticationBackgroundJob, AuthenticationBackgroundJobService>();
            builder.Services.AddScoped<ITodoBackgroundJob, TodoBackgroundJob>();
            builder.Services.AddScoped<IAuthentications, AuthenticationServices>();
            builder.Services.AddScoped<TodoRecurring>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddScoped<ITodoService, ToDoService>();
            builder.Services.AddScoped<IFileService, FileUploadService>();
            builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
            builder.Services.AddSingleton<ISpecificationBuilderFactory, SpeificationBuilderFactory>();


            // ============================
            // CORS
            // ============================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("ToDoApplicationPolicies", policy =>
                {
                    policy
                    .WithOrigins(
                        "https://localhost:7259",
                        "http://localhost:5173"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });



            // ============================
            // JWT Authentication
            // ============================
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine(
                                "JWT ERROR : " +
                                context.Exception.Message
                            );

                            return Task.CompletedTask;
                        },

                        OnForbidden = context =>
                        {
                            Console.WriteLine(
                                "JWT FORBIDDEN"
                            );

                            return Task.CompletedTask;
                        }
                    };


                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,

                            ValidateAudience = true,

                            ValidateLifetime = true,

                            ValidateIssuerSigningKey = true,

                            ClockSkew = TimeSpan.Zero,


                            ValidIssuer =
                                builder.Configuration["JWT:Issuer"],


                            ValidAudience =
                                builder.Configuration["JWT:Audience"],


                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(
                                        builder.Configuration["JWT:Key"]!
                                    )
                                )
                        };
                });



            // ============================
            // Swagger + JWT
            // ============================
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",

                        Type = SecuritySchemeType.Http,

                        Scheme = "Bearer",

                        BearerFormat = "JWT",

                        In = ParameterLocation.Header,

                        Description =
                            "Enter: Bearer {your JWT token}"
                    }
                );


                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference =
                                    new OpenApiReference
                                    {
                                        Type =
                                            ReferenceType.SecurityScheme,

                                        Id = "Bearer"
                                    }
                            },

                            Array.Empty<string>()
                        }
                    }
                );
            });



            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                await IdentitySeed.SeedRolesAsync(
                    scope.ServiceProvider);
            }

            // ============================
            // Middleware Pipeline
            // ============================

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI();
            }

            app.UseHangfireDashboard();
            app.UseHttpsRedirection();


            app.UseStaticFiles();


            app.UseCors(
                "ToDoApplicationPolicies"
            );


            app.UseAuthentication();


            app.UseAuthorization();



            app.MapControllers();

            RecurringJob.AddOrUpdate<TodoRecurring>(
                            "enqueue-recurring-todos",
                            job => job.EnqueuJobRecurringTodo(),
                            Cron.MinuteInterval(1)
                        );

            await app.RunAsync();
        }
    }
}

