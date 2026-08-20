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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(x => x.UserName)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasOne(x => x.User)
                .WithMany(x => x.Categories)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ToDoItem>()
                .HasOne(x => x.User)
                .WithMany(x => x.ToDoItems)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ToDoItem>()
                .HasOne(x => x.Category)
                .WithMany(x => x.ToDoItems)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Category>()
           .HasIndex(c => new { c.UserId, c.Title })
           .IsUnique();

            modelBuilder.Entity<Role>()
                .HasData(
                    new Role
                    {
                        Id = enumUserRole.UserAdmin,
                        RoleName = "Admin"
                    },
                    new Role
                    {
                        Id = enumUserRole.User,
                        RoleName = "User"
                    });

            modelBuilder.Entity<FilesUpload>()
                .HasIndex(x => x.Path);
        }
    }
}
