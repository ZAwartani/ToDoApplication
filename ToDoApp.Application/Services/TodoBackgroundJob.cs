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
        private readonly ITodoService _TodoService;
        private readonly ISpecificationBuilderFactory Factory;
        public TodoBackgroundJob(IGenericRepository<RecurringTodo , int> RecurringTodoRepository 
            , ISpecificationBuilderFactory specificationBuilderFactory
            , ITodoService  todoService
            , IGenericRepository<ToDoItem, int> todoRepository
            )
        {
            _todoRepository = todoRepository;
            _TodoService = todoService;
            _RecurringTodoRepository = RecurringTodoRepository;
            Factory = specificationBuilderFactory;
        }

        public async Task AddRecurringTodoAsync(ReturnTodoItem returnTodoItem)
        {
            var nextOccurrence = returnTodoItem.Recurring switch
            {
                EnumRecurring.Daily => DateTime.Now.AddMinutes(3),
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

            var newTodo = new CreateTodoDto(
                originalTodo.Title,
                originalTodo.Description,
                originalTodo.Priority,
                (DateTime)originalTodo.ExpireDate,
                originalTodo.CategoryId,
                null,
                EnumCompleteStatus.NotComplete
            );

            await _TodoService.Create(newTodo);

            var nextOccurrence = originalTodo.RecurringSetting switch
            {
                EnumRecurring.Daily => DateTime.Now.AddMinutes(3),
                EnumRecurring.Weekly => DateTime.Now.AddDays(7),
                EnumRecurring.Monthly => DateTime.Now.AddMonths(1),
                _ => (DateTime?)null
            };

            if (nextOccurrence.HasValue)
            {
                recurringTodo.OccurrenceDate = nextOccurrence.Value;
                await _RecurringTodoRepository.UpdateAsync(recurringTodo);
            }
            else
            {
                await _RecurringTodoRepository.DeleteAsync(recurringTodo.Id);
            }
        }
    }
}
