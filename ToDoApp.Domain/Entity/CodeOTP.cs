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
        public string? Codeotp { get; set; }
        public DateTime? CodeOTPExpir { get; set; }

        [ForeignKey(nameof(Users))]
        public int UserId { get; set; }
        public Users Users { get; set; }
        public CodeOTP(string codeotp, DateTime? codeOTPExpir, int userId)
        {
            Codeotp = codeotp;
            CodeOTPExpir = codeOTPExpir;
            UserId = userId;
        }
    }
}
