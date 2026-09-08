using Hangfire;
using ToDoApp.Application.Interfaces.BackgroundJobs;
using ToDoApp.Application.Interfaces.Factory;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Entity;

namespace TodoApp.Presentation.BackgroundJobs
{
    public class TodoRecurring
    {
        private readonly IGenericRepository<RecurringTodo, int> _RecurringTodoRepository;
        private readonly ISpecificationBuilderFactory _factory;
        private readonly ITodoBackgroundJob _todoBackgroundJob;

        public TodoRecurring(
            IGenericRepository<RecurringTodo, int> recurringTodoRepository,
            ISpecificationBuilderFactory factory,
            ITodoBackgroundJob todoBackgroundJob)
        {
            _RecurringTodoRepository = recurringTodoRepository;
            _factory = factory;
            _todoBackgroundJob = todoBackgroundJob;
        }

        public async Task EnqueuJobRecurringTodo()
        {
            while (true)
            {
                var specification = _factory
                    .CreateBuilder<RecurringTodo>()
                    .AddCriteria(x =>
                        x.OccurrenceDate <= DateTime.Now)
                    .ApplyOrderBy(x => x.OccurrenceDate)
                    .ApplyPaging(1, 10);

                var recurringTodos =
                    await _RecurringTodoRepository.ListAsync(specification);

                if (recurringTodos.Count == 0)
                {
                    break;
                }

                foreach (var recurringTodo in recurringTodos)
                {

                    await _todoBackgroundJob.ExecuteRecurringTodoAsync(recurringTodo.Id);
                }
                await _RecurringTodoRepository.UpdateRangeAsync(recurringTodos);
            }
        }
    }
}