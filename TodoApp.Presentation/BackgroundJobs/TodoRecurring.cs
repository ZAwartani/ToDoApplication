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

        public TodoRecurring(
            IGenericRepository<RecurringTodo, int> recurringTodoRepository,
            ISpecificationBuilderFactory factory)
        {
            _RecurringTodoRepository = recurringTodoRepository;
            _factory = factory;
        }

        public async Task EnqueuJobRecurringTodo()
        {
            while (true)
            {
                var specification = _factory
                    .CreateBuilder<RecurringTodo>()
                    .AddCriteria(x =>
                        x.OccurrenceDate <= DateTime.Now &&
                        !x.IsSend)
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
                    recurringTodo.UpdateSend(true);
                    await _RecurringTodoRepository.UpdateAsync(recurringTodo);

                    BackgroundJob.Enqueue<ITodoBackgroundJob>(
                        x => x.ExecuteRecurringTodoAsync(recurringTodo.Id)
                    );
                }

                if (recurringTodos.Count < 10)
                {
                    break;
                }
            }
        }
    }
}
