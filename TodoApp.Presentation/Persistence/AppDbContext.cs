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

        DbSet<User> Users { get; set; }
        DbSet<CodeOTP> codeOTPs { get; set; }
        DbSet<Token> tokens { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<ToDoItem> ToDoItems { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<FilesUpload> FilesUploads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();

            modelBuilder.Entity<Role>()
               .HasData(
                        new Role { Id = enumUserRole.UserAdmin, RoleName = "Admin" },
                        new Role { Id = enumUserRole.User, RoleName = "User" }
                );

            modelBuilder.Entity<Category>().HasData(new { Id = 1 , Title = "UnCategorical"});
            modelBuilder.Entity<FilesUpload>().HasIndex(x => x.Path);
            base.OnModelCreating(modelBuilder);

        }
    }
}
