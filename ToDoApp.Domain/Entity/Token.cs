using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ToDoApp.Domain.Interfaces;

namespace ToDoApp.Domain.Entity
{
    public class Token : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string RefreshToken { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime RefreshTokenExpir { get; set; }
        public DateTime? RefreshTokenRevokedAt { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }

        private Token() { }
        public Token(int UserId , string RefreshToken , DateTime RefreshExpire , DateTime? RefreshTokenRevokedAt , DateTime CreatedDate) { 
            this.UserId = UserId;
            this.RefreshToken = RefreshToken;
            this.RefreshTokenExpir = RefreshExpire;
            this.RefreshTokenRevokedAt = RefreshTokenRevokedAt;
            this.CreatedDate = CreatedDate;
        }
        public Token(int UserId)
        {
            this.UserId = UserId;
        }
        public Token(string RefreshToken , DateTime CreatedDate, DateTime RefreshExpire , DateTime? RefreshTokenRevoked) 
        {
            this.RefreshToken = RefreshToken;
            this.CreatedDate = CreatedDate;
            this.RefreshTokenExpir = RefreshExpire;
            this.RefreshTokenRevokedAt = RefreshTokenRevoked;
        }
    }
}
