using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ToDoApp.Domain.Enums;
using ToDoApp.Domain.Interfaces;

namespace ToDoApp.Domain.Entity
{
    public class Users : IEntity<int>
    {
        [Key]
        public int Id { get; set; } //
        public string UserName  { get; set; }//
        public string Email { get; set; }//
        public string Password { get; set; }//
        public DateTime CreatedAt { get; set; }//


        [ForeignKey(nameof(Roles))]
        public enumUserRole RoleId { get; set; }
        public Roles Roles { get; set; }
        
        public ICollection<CodeOTP> CodeOTPs { get; set; }
        public ICollection<ToDoItem> ToDoItems{ get; set; }

        public Users()
        {
            
        }
        public Users(string UserName , string Email)
        {
            this.UserName = UserName;
            this.Email = Email;
            this.RoleId = enumUserRole.User;
        }
        public Users(string UserName , string Email , string Password , enumUserRole Roles )
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {
                throw new ArgumentException("They Have Error !!");
            }
            this.UserName = UserName;
            this.Email = Email; 
            this.Password = Password;
            this.RoleId = Roles;
            this.CreatedAt = DateTime.UtcNow;
        }

        public void UpdatePasswordByAdmin(string Password)
        {
            this.Password = Password;
        }
    }
}
