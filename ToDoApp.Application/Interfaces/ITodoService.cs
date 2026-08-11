using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.DTOs;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces
{
    public interface ITodoService
    {
        public Task<bool> CreateTodo(CreateTodoDto Dto , int UserId);
        public Task<bool> UpdateTodo(UpdateTodoDto Dto , int UserId , int Id);
        public Task<bool> DeleteTodo(int Id , int UserId);
        public Task<bool> MarkTodoIsComplete(MarkAsComplete Dto , int Id , int UserId);
        public Task<IReadOnlyList<ReturnTodoItem>> ListTodoItemAsync(int UserId);
    }
}
