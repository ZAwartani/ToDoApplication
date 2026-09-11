using System;
using System.Collections.Generic;
using System.Text;

namespace ToDoApp.Application.Interfaces
{
    public interface ICurrentUser
    {
        public int UserId { get;}
        string Email { get; }
        string Role { get; }
        bool IsAuthenticated { get; }

        bool IsInRole(string role);
    }
}
