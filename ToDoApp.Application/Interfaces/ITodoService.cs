using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.Dtos;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Entity;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Interfaces
{
    public interface ITodoService
    {
        public Task<bool> CreateAsync(CreateTodoDto Dto);
        public Task<bool> UpdateAsync(UpdateTodoDto Dto );
        public Task<bool> DeleteAsync(int Id);
        public Task<bool> MarkAsCompleteAsync(MarkAsCompleteDto Dto);
        public Task<IReadOnlyList<ReturnTodoItemDto>> ListAsync(string? FilterBy , bool Descending);
        public Task SendEmailReminderAsync(string Title, DateTime? ExpireDate);
        public Task<bool> UpdateTodoForUserAsync(UpdateTodoDto UpdateTodoDto);

    }
}
