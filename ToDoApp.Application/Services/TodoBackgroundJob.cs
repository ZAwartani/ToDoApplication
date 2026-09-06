using Org.BouncyCastle.Crypto.Signers;
using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.BackgroundJobs;
using ToDoApp.Application.Interfaces.Factory;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Entity;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Services
{
    public class TodoBackgroundJob : ITodoBackgroundJob
    {
        private readonly IGenericRepository<RecurringTodo , int> _RecurringTodoRepository;
        private readonly IGenericRepository<ToDoItem, int> _todoRepository;
        private readonly ISpecificationBuilderFactory Factory;
        public TodoBackgroundJob(IGenericRepository<RecurringTodo , int> RecurringTodoRepository 
            , ISpecificationBuilderFactory specificationBuilderFactory
            , IGenericRepository<ToDoItem, int> todoRepository
            )
        {
            _todoRepository = todoRepository;
            _RecurringTodoRepository = RecurringTodoRepository;
            Factory = specificationBuilderFactory;
        }

        public async Task AddRecurringTodoAsync(ReturnTodoItem returnTodoItem)
        {
            var nextOccurrence = returnTodoItem.Recurring switch
            {
                EnumRecurring.Daily => DateTime.Now.AddDays(1),
                EnumRecurring.Weekly => DateTime.Now.AddDays(7),
                EnumRecurring.Monthly => DateTime.Now.AddMonths(1),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(returnTodoItem.Recurring),
                    returnTodoItem.Recurring,
                    "Unsupported recurrence type.")
            };

            var recurringTodo = new RecurringTodo(returnTodoItem.Id, nextOccurrence);

            await _RecurringTodoRepository.AddAsync(recurringTodo);
        }

        public async Task ExecuteRecurringTodoAsync(int recurringTodoId)
        {
            var recurringTodo = await _RecurringTodoRepository.GetByIdAsync(recurringTodoId);

            if (recurringTodo == null)
            {
                return;
            }

            var originalTodo = await _todoRepository.GetByIdAsync(recurringTodo.TodoId);

            if (originalTodo == null)
            {
                await _RecurringTodoRepository.DeleteAsync(recurringTodo.Id);
                return;
            }

            var newTodo = new ToDoItem(
                originalTodo.Title,
                originalTodo.Description,
                originalTodo.Priority,
                originalTodo.UserId,
                (DateTime)originalTodo.ExpireDate,
                originalTodo.CategoryId,
                originalTodo.RecurringSetting
            );

            await _todoRepository.AddAsync(newTodo);

            var nextOccurrence = originalTodo.RecurringSetting switch
            {
                EnumRecurring.Daily => DateTime.Now.AddDays(1),
                EnumRecurring.Weekly => DateTime.Now.AddDays(7),
                EnumRecurring.Monthly => DateTime.Now.AddMonths(1),
                _ => (DateTime?)null
            };

            if (nextOccurrence.HasValue)
            {
                recurringTodo.OccurrenceDate = nextOccurrence.Value;
                recurringTodo.UpdateSend(true);
                await _RecurringTodoRepository.UpdateAsync(recurringTodo);
            }
            else
            {
                await _RecurringTodoRepository.DeleteAsync(recurringTodo.Id);
            }
        }
    }
}
