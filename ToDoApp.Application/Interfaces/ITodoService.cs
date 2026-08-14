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
        public Task<bool> Create(CreateTodoDto Dto , int UserId);
        public Task<bool> Update(UpdateTodoDto Dto , int UserId , int Id);
        public Task<bool> Delete(int Id , int UserId);
        public Task<bool> MarkAsComplete(MarkAsComplete Dto , int Id , int UserId);
        public Task<IReadOnlyList<ReturnTodoItem>> ListAsync(int UserId , string? FilterBy , bool Descending);
        public Task SendEmailReminderAsync(int userId, string title, DateTime? expireDate);
        public Task CreateRecurringTodoAsync(CreateTodoDto dto ,int UesrId ,EnumRecurring enumRecurring);
        Task<bool> UpdateTodoForUser(UpdateTodoDto updateTodoDto, int Id);

    }
}
