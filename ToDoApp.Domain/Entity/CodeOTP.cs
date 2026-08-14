using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ToDoApp.Domain.Interfaces;

namespace ToDoApp.Domain.Entity
{
    public class CodeOTP : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string? Code { get; set; }
        public DateTime? CodeExpirDate { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }
        public CodeOTP(string code, DateTime? codeExpirDate, int userId)
        {
            Code = code;
            CodeExpirDate = codeExpirDate;
            UserId = userId;
        }
    }
}
