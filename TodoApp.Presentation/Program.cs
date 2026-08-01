using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TodoApp.Presentation.Persistence;
using TodoApp.Presentation.Repositories;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Application.Services;

namespace TodoApp.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // ============================
            // Controllers
            // ============================
            builder.Services.AddControllers();



            // ============================
            // Database
            // ============================
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("MyConnection")
                );
            });



            // ============================
            // Dependency Injection
            // ============================
            builder.Services.AddScoped(
    typeof(IGenericRepository<,>),
    typeof(GenericRepository<,>)
    );

            builder.Services.AddScoped<IAuthentications, AuthenticationServices>();
            
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IAdminService, AdminService>();

            builder.Services.AddScoped<ITodoService, ToDoService>();



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



            // ============================
            // Middleware Pipeline
            // ============================

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI();
            }


            app.UseHttpsRedirection();


            app.UseStaticFiles();


            app.UseCors(
                "ToDoApplicationPolicies"
            );


            app.UseAuthentication();


            app.UseAuthorization();



            app.MapControllers();


            app.Run();
        }
    }
}