using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Entity;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Interfaces
{
    public interface ITodoService
    {
        public Task<bool> Create(CreateTodoDto Dto);
        public Task<bool> Update(UpdateTodoDto Dto , int Id);
        public Task<bool> Delete(int Id);
        public Task<bool> MarkAsComplete(MarkAsComplete Dto , int Id);
        public Task<IReadOnlyList<ReturnTodoItem>> ListAsync(string? FilterBy , bool Descending);
        public Task SendEmailReminderAsync(string Title, DateTime? ExpireDate);
        public Task<bool> UpdateTodoForUser(UpdateTodoDto UpdateTodoDto, int Id);

    }
}
