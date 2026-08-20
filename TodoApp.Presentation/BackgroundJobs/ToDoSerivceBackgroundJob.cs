using Hangfire;
using System.Security.Cryptography;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Services;
using ToDoApp.Domain.Enums;

namespace TodoApp.Presentation.BackgroundJobs
{
    public class ToDoSerivceBackgroundJob : ITodoJobs
    {
        private readonly ToDoService _TodoRepo;
        public ToDoSerivceBackgroundJob(ToDoService TodoRepo)
        {
            this._TodoRepo = TodoRepo;
        }

        public async Task MakeRecurringTodo(CreateTodoDto dto, int UserId)
        {
            if (dto.Recurring == null)
            {
                if (dto.Recurring != null)
                {
                    if (dto.Recurring == EnumRecurring.Daily)
                    {
                        RecurringJob.AddOrUpdate("daily-recurring", () => _TodoRepo.CreateRecurringTodoAsync(dto, UserId, EnumRecurring.Daily), Cron.Daily);
                    }
                    else if (dto.Recurring == EnumRecurring.Weekly)
                    {
                        RecurringJob.AddOrUpdate("weekly-recurring", () => _TodoRepo.CreateRecurringTodoAsync(dto, UserId, EnumRecurring.Weekly), Cron.Weekly);
                    }
                    else if (dto.Recurring == EnumRecurring.Monthly)
                    {
                        RecurringJob.AddOrUpdate("monthly-recurring", () => _TodoRepo.CreateRecurringTodoAsync(dto, UserId, EnumRecurring.Monthly), Cron.Monthly);
                    }
                }
            }
        }

        public async Task RemoveSchedualJob(int TodoId)
        {
            string TitleBackground = await _TodoRepo.GetTitleOfJob(TodoId);
            BackgroundJob.Delete(TitleBackground);
        }

        public async Task<string> SendEmailReminderForTodo(int UserId, string Title, DateTime ExpireDate)
        {
            var TitleBackground = BackgroundJob.Schedule(() => _TodoRepo.SendEmailReminderAsync(UserId, Title, ExpireDate), ExpireDate.AddDays(-1));
            return TitleBackground;
        }

        Task ITodoJobs.MakeRecurringTodo(CreateTodoDto dto, int UserId)
        {
            return MakeRecurringTodo(dto, UserId);
        }
    }
}
