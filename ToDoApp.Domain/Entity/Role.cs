using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ToDoApp.Domain.Enums;
using ToDoApp.Domain.Interfaces;

namespace ToDoApp.Domain.Entity;
    public class Role : IEntity<int>
    {
        [Key]
        public enumUserRole Id { get; set; }
        public string RoleName { get; set; }
        public ICollection<User> Users { get; set; }
    }
