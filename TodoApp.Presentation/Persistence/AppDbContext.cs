using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Mail;
using ToDoApp.Domain.Entity;
using ToDoApp.Domain.Enums;

namespace TodoApp.Presentation.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        DbSet<Users> Users { get; set; }
        DbSet<CodeOTP> codeOTPs { get; set; }
        DbSet<Token> tokens { get; set; }
        DbSet<Roles> Roles { get; set; }
        DbSet<ToDoItem> ToDoItems { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<FilesUpload> FilesUploads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Users>().HasIndex(u => u.UserName).IsUnique();

            modelBuilder.Entity<Roles>()
               .HasData(
                        new Roles { Id = enumUserRole.UserAdmin, RoleName = "Admin" },
                        new Roles { Id = enumUserRole.User, RoleName = "User" }
                );

            modelBuilder.Entity<ToDoItem>().HasIndex(x => x.Title).IsUnique();
            modelBuilder.Entity<ToDoItem>().HasIndex(x => new { x.UserId, x.Title }).IsUnique();
            modelBuilder.Entity<Category>().HasIndex(x => x.TitleOfCategory).IsUnique();
            modelBuilder.Entity<Category>().HasData(new { Id = 1 , TitleOfCategory = "UnCategorical"});
            modelBuilder.Entity<FilesUpload>().HasIndex(x => x.AddressNameFile).IsUnique();
            base.OnModelCreating(modelBuilder);


        }
    }
}
