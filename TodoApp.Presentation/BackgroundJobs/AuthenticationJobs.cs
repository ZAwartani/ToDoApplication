using Hangfire;
using ToDoApp.Application.Dtos;
using ToDoApp.Application.Interfaces.BackgroundJob;

namespace TodoApp.Presentation.BackgroundJobs
{
    public class AuthenticationJobs
    {
        public Task SendOTPAsync(SendOTP SendOTPDto)
        {
           BackgroundJob.Enqueue<IAuthenticationBackgroundJob>(job => job.SendOTPAsync(SendOTPDto));
            return Task.CompletedTask;
        }
    }
}
