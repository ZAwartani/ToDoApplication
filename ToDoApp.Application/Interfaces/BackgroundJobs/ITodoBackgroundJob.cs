using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.Dtos;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces.BackgroundJobs
{
    public interface ITodoBackgroundJob
    {
        public Task AddRecurringTodoAsync(ReturnTodoItemDto ReturnTodoItem);
        public Task ExecuteRecurringTodoAsync(int Id);
    }
}
