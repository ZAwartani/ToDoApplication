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
        public DateTime RefreshTokenExpir { get; set; }
        public DateTime? RefreshTokenRevokedAt { get; set; }

        [ForeignKey(nameof(Users))]
        public int UserId { get; set; }
        public Users Users { get; set; }

        public Token() { }
        public Token(int UserId)
        {
            this.UserId = UserId;
        }
        public Token(string RefreshToken , DateTime RefreshExpire , DateTime? RefreshTokenRevoked) 
        {
            this.RefreshToken = RefreshToken;
            this.RefreshTokenExpir = RefreshExpire;
            this.RefreshTokenRevokedAt = RefreshTokenRevoked;
        }
    }
}
