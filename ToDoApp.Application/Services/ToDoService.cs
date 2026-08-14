using MailKit.Net.Smtp;
using MimeKit;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.Factory;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Entity;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Services
{
    public class ToDoService : ITodoService
    {
        private readonly IGenericRepository<ToDoItem, int> _RepoTodo;
        private readonly IGenericRepository<Category, int> _RepoCategory;
        private readonly IGenericRepository<User, int> _RepoUser;
        private readonly ISpecificationBuilderFactory _Factory;


        public ToDoService(
            IGenericRepository<ToDoItem, int> genericRepositoryTodoItem,
            IGenericRepository<Category, int> genericRepositoryCategory,
            IGenericRepository<User, int> genericRepositoryUser,
            ISpecificationBuilderFactory factory
            )
        {
            this._RepoTodo = genericRepositoryTodoItem;
            this._RepoCategory = genericRepositoryCategory;
            this._Factory = factory;
            this._RepoUser = genericRepositoryUser;
        }
        public async Task<bool> UpdateTodoForUser(UpdateTodoDto UpdateTodoDto, int Id)
        {
            var CategoryId = UpdateTodoDto.CategoryId ?? Constants.DefaultCategoryId;

            var ToDo = await _RepoTodo.GetByIdAsync(Id);

            if (ToDo == null)
                return false;

            ToDo.Update(
                UpdateTodoDto.Title,
                UpdateTodoDto.Description,
                UpdateTodoDto.Priority,
                UpdateTodoDto.ExpireDate,
                CategoryId,
                UpdateTodoDto.Recurring
            );

            await _RepoTodo.UpdateAsync(ToDo);
            return true;
        }
        public async Task<bool> Create(CreateTodoDto dto, int userId)
        {
            var CategoryId = dto.CategoryId ?? Constants.DefaultCategoryId;

            var CategorySpec = _Factory.CreateBuilder<Category>()
                .AddCriteria(x => x.Id == CategoryId);
 
                var Category = await _RepoCategory.GetAsync(CategorySpec);

            if (Category == null)
                throw new Exception("Category not found");

            var todo = new ToDoItem(
                dto.Title,
                dto.Description,
                dto.Priority,
                userId,
                dto.ExpireDate,
                CategoryId,
                dto.Recurring
            );

            await _RepoTodo.AddAsync(todo);

            return true;
        }

        public async Task<IReadOnlyList<ReturnTodoItem>> ListAsync(int UserId,string? FilterBy,bool Descending)
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

            var result = await _RepoTodo.ListAsync(Spec);

            if (result == null || result.Count == 0)
                throw new Exception("User doesn't have any TodoItems");

            var mapped = result.Select(x => new ReturnTodoItem(
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

            return mapped;
        }

        public async Task<bool> Update(
            UpdateTodoDto dto,
            int Id,
            int userId)
        {
            var ToDo = await _RepoTodo.GetByIdAsync(Id);

            if ( ToDo == null || ToDo.UserId != userId)
                return false;

            var CategoryId = dto.CategoryId ?? Constants.DefaultCategoryId;

            ToDo.Update(
                dto.Title,
                dto.Description,
                dto.Priority,
                dto.ExpireDate,
                CategoryId,
                dto.Recurring
            );


            await _RepoTodo.UpdateAsync(ToDo);
            return true;
        }
        public async Task<bool> Delete(
            int Id,
            int userId)
        {

            var ToDo = await _RepoTodo
                .GetByIdAsync(Id);


            if (ToDo == null)
                return false;

            if (ToDo.UserId != userId)
                return false;

            await _RepoTodo.DeleteAsync(Id);
            return true;
        }

        public async Task<bool> MarkAsComplete(
            MarkAsComplete dto,
            int Id,
            int userId)
        {

            var ToDo = await _RepoTodo
                .GetByIdAsync(Id);


            if (ToDo == null)
                return false;

            if (ToDo.UserId != userId)
                return false;

            ToDo.UpdateStatus(dto.IsComplete);

            await _RepoTodo.UpdateAsync(ToDo);

            return true;
        }

        public async Task SendEmailReminderAsync(int UserId, string Title, DateTime? ExpireDate)
        {
            var spec = _Factory.CreateBuilder<User>()
                .AddCriteria(u => u.Id == UserId);

            var user = await _RepoUser.GetAsync(spec);

            if (user == null)
                throw new Exception("User not found");

            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("TodoApp", "zakaryawork005@gmail.com"));
            message.To.Add(new MailboxAddress("User", user.Email));
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

        public async Task CreateRecurringTodoAsync(CreateTodoDto dto, int userId, EnumRecurring enumRecurring)
        {

            var CategoryId = dto.CategoryId ?? Constants.DefaultCategoryId;

            if (enumRecurring == EnumRecurring.Daily)
                {
                    var todo = new ToDoItem(
                        dto.Title,
                        dto.Description,
                        dto.Priority,
                        userId,
                        dto.ExpireDate,
                        CategoryId,
                        EnumRecurring.Daily
                    );
                    _RepoTodo.AddAsync(todo);
                }
                else if (enumRecurring == EnumRecurring.Weekly)
                {
                    var todo = new ToDoItem(
                        dto.Title,
                        dto.Description,
                        dto.Priority,
                        userId,
                        dto.ExpireDate,
                        CategoryId,
                        EnumRecurring.Weekly
                    );
                    _RepoTodo.AddAsync(todo);
                }
                else if (enumRecurring == EnumRecurring.Monthly)
                {
                    var todo = new ToDoItem(
                        dto.Title,
                        dto.Description,
                        dto.Priority,
                        userId,
                        dto.ExpireDate,
                        CategoryId,
                        EnumRecurring.Monthly
                    );
                    _RepoTodo.AddAsync(todo);
                }
            }
        }
     
    internal static class Constants
    {
        public const int DefaultCategoryId = 1;

    }
}
