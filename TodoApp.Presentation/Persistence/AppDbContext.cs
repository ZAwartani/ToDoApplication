using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Mail;
using ToDoApp.Domain.Entity;
using ToDoApp.Domain.Enums;

namespace TodoApp.Presentation.Persistence
{
    public class AppDbContext : IdentityDbContext<User , Role , int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<CodeOTP> codeOTPs { get; set; }
        public DbSet<Token> tokens { get; set; }
        public DbSet<ToDoItem> ToDoItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<FilesUpload> FilesUploads { get; set; }
        public DbSet<RecurringTodo> RecurringTodos { get; set; }

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

            modelBuilder.Entity<RecurringTodo>()
                .HasOne(x => x.Todo)
                .WithMany(x => x.RecurringTodos)
                .HasForeignKey(x => x.TodoId)
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

            modelBuilder.Entity<FilesUpload>()
                .HasIndex(x => x.Path);
        }
    }
}
