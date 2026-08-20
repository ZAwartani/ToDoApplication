using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.DTOs;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Interfaces
{
    public interface ITodoJobs
    {
        public Task<string> SendEmailReminderForTodo(int UserId, string Title, DateTime ExpireDate);
        public Task MakeRecurringTodo(CreateTodoDto dto , int UserId );
        public Task RemoveSchedualJob(int TodoId);

    }
}
