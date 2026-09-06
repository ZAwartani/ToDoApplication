using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.DTOs;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces.BackgroundJobs
{
    public interface ITodoBackgroundJob
    {
        public Task AddRecurringTodoAsync(ReturnTodoItem ReturnTodoItem);
        public Task ExecuteRecurringTodoAsync(int recurringTodoId);
    }
}
