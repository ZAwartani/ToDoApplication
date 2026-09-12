using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ToDoApp.Domain.Enums;
using ToDoApp.Domain.Interfaces;

namespace ToDoApp.Domain.Entity
{
    public class User : IdentityUser<int>, IEntity<int>
    {
        public DateTime CreatedAt { get; set; }
        public ICollection<CodeOTP> Codes { get; set; } = new List<CodeOTP>();
        public ICollection<ToDoItem> ToDoItems { get; set; } = new List<ToDoItem>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();

        protected User() { }

        public User(string userName, string email) : base(userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("UserName is required.", nameof(userName));

            Email = email;
            CreatedAt = DateTime.UtcNow;
        }
    }
}