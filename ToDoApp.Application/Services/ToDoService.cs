using MailKit.Net.Smtp;
using MimeKit;
using Org.BouncyCastle.Crypto.Signers;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.BackgroundJobs;
using ToDoApp.Application.Interfaces.Factory;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Entity;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Services
{
    public class ToDoService : ITodoService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IGenericRepository<ToDoItem, int> _RepoTodo;
        private readonly ITodoBackgroundJob _TodoBackgroundJob;
        private readonly IGenericRepository<Category, int> _RepoCategory;
        private readonly IGenericRepository<User, int> _RepoUser;
        private readonly ISpecificationBuilderFactory _Factory;


        public ToDoService(
            IGenericRepository<ToDoItem, int> genericRepositoryTodoItem,
            IGenericRepository<Category, int> genericRepositoryCategory,
            IGenericRepository<User, int> genericRepositoryUser,
            ISpecificationBuilderFactory factory,
            ITodoBackgroundJob todoBackgroundJob,
            ICurrentUser _currentUser
            )
        {
            this._currentUser = _currentUser;
            this._RepoTodo = genericRepositoryTodoItem;
            this._RepoCategory = genericRepositoryCategory;
            this._Factory = factory;
            this._RepoUser = genericRepositoryUser;
            this._TodoBackgroundJob = todoBackgroundJob;
        }
        private int UserId => _currentUser.UserId;
        public async Task<bool> UpdateTodoForUser(UpdateTodoDto UpdateTodoDto, int Id)
        {

            var ToDo = await _RepoTodo.GetByIdAsync(Id);

            if (ToDo == null)
                return false;

            ToDo.Update(
                UpdateTodoDto.Title,
                UpdateTodoDto.Description,
                UpdateTodoDto.Priority,
                UpdateTodoDto.ExpireDate,
                UpdateTodoDto.CategoryId,
                UpdateTodoDto.Recurring
            );

            await _RepoTodo.UpdateAsync(ToDo);
            return true;
        }
        public async Task<bool> Create(CreateTodoDto Dto)
        {
            ToDoItem Todo;
            if (Dto.CategoryId == null)
            {
                Todo = new ToDoItem(
                    Dto.Title,
                    Dto.Description,
                    Dto.Priority,
                    UserId,
                    Dto.ExpireDate,
                    null,
                    Dto.Recurring
                );
            }

            else
            {

                var CategorySpec = _Factory.CreateBuilder<Category>()
                    .AddCriteria(x => x.UserId == UserId)
                    .AddCriteria(x => x.Id == Dto.CategoryId);

                var Category = await _RepoCategory.GetAsync(CategorySpec);

                if (Category != null)
                {
                    Todo = new ToDoItem(
                           Dto.Title,
                           Dto.Description,
                           Dto.Priority,
                           UserId,
                           Dto.ExpireDate,
                           Dto.CategoryId,
                           Dto.Recurring
                    );
                }
                else
                {
                    throw new Exception("We have Problem");
                }
            }
            await _RepoTodo.AddAsync(Todo);
            if(Todo.RecurringSetting != null)
            {
                var TodoReturn = new ReturnTodoItem(
                    Todo.Id,
                    Todo.Title,
                    Todo.Description,
                    Todo.Priority,
                    Todo.CreatedAt,
                    Todo.ExpireDate,
                    Todo.CategoryId,
                    Todo.IsComplete,
                    Todo.RecurringSetting
                );
                await _TodoBackgroundJob.AddRecurringTodoAsync(TodoReturn);
            }
            return true;
        }

        public async Task<IReadOnlyList<ReturnTodoItem>> ListAsync(string? FilterBy, bool Descending)
        {

            var Spec = _Factory.CreateBuilder<ToDoItem>()
                .AddCriteria(x => x.UserId == UserId);

            if (!string.IsNullOrWhiteSpace(FilterBy))
            {
                if (FilterBy.Equals("priority", StringComparison.OrdinalIgnoreCase))
                {
                    Spec.ApplyOrderBy(x => x.Priority, Descending);
                }
                else if (FilterBy.Equals("expiredate", StringComparison.OrdinalIgnoreCase))
                {
                    Spec.ApplyOrderBy(x => x.ExpireDate, Descending);
                }
                else if (FilterBy.Equals("category", StringComparison.OrdinalIgnoreCase))
                {
                    Spec.ApplyOrderBy(x => x.CategoryId, Descending);
                }
                else if (FilterBy.Equals("createdat", StringComparison.OrdinalIgnoreCase))
                {
                    Spec.ApplyOrderBy(x => x.CreatedAt, Descending);
                }
                else if (FilterBy.Equals("iscomplete", StringComparison.OrdinalIgnoreCase))
                {
                    Spec.ApplyOrderBy(x => x.IsComplete, Descending);
                }
            }

            var Result = await _RepoTodo.ListAsync(Spec);

            if (Result == null ||  Result.Count == 0)
                throw new Exception("User doesn't have any TodoItems");

            var Mapped = Result.Select(x => new ReturnTodoItem(
                x.Id,
                x.Title,
                x.Description,
                x.Priority,
                x.CreatedAt,
                x.ExpireDate,
                x.CategoryId,
                x.IsComplete,
                x.RecurringSetting
            )).ToList();

            return Mapped;
        }

        public async Task<bool> Update(
            UpdateTodoDto Dto,
            int Id)
        {

            var ToDo = await _RepoTodo.GetByIdAsync(Id);

            if (ToDo == null || ToDo.UserId != UserId)
                return false;

            ToDo.Update(
                Dto.Title,
                Dto.Description,
                Dto.Priority,
                Dto.ExpireDate,
                Dto.CategoryId,
                Dto.Recurring
            );
            await _RepoTodo.UpdateAsync(ToDo);
            return true;
        } // Checked

        public async Task<bool> Delete(
            int Id)
        {

            var ToDo = await _RepoTodo
                .GetByIdAsync(Id);


            if (ToDo == null)
                return false;

            if (ToDo.UserId != UserId)
                return false;

            await _RepoTodo.DeleteAsync(Id);
            return true;
        }

        public async Task<bool> MarkAsComplete(
            MarkAsComplete Dto,
            int Id)
        {

            var ToDo = await _RepoTodo
                .GetByIdAsync(Id);


            if (ToDo == null)
                return false;

            if (ToDo.UserId != UserId)
                return false;

            ToDo.UpdateStatus(Dto.IsComplete);

            await _RepoTodo.UpdateAsync(ToDo);

            return true;
        }

        public async Task SendEmailReminderAsync(string Title, DateTime? ExpireDate)
        {

            var Spec = _Factory.CreateBuilder<User>()
                .AddCriteria(u => u.Id == UserId);

            var User = await _RepoUser.GetAsync(Spec);

            if (User == null)
                throw new Exception("User not found");

            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("TodoApp", "zakaryawork005@gmail.com"));
            message.To.Add(new MailboxAddress("User", User.Email));
            message.Subject = "Todo Reminder";

            message.Body = new TextPart("plain")
            {
                Text = $"The Todo Item : {Title}. Is Expire Tomorrow {ExpireDate?.Date} "
            };


            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("zakaryawork005@gmail.com", "qkukqdjeaqbxkjod");
                    await client.SendAsync(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"حدث خطأ أثناء الإرسال: {ex.Message}");
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
            Console.WriteLine($"Sending email reminder to user {UserId} for todo '{Title}' expiring on {ExpireDate}");
        }
    }
}
