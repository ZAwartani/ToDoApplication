using System;
using System.Collections.Generic;
using System.Text;

namespace ToDoApp.Application.Interfaces
{
    public interface ICurrentUser
    {
        public int Id { get;}
        string Email { get; }
        string Role { get; }
        bool IsAuthenticated { get; }

        bool IsInRole(string role);
    }
}
