using Hangfire;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces.BackgroundJob;

namespace TodoApp.Presentation.BackgroundJobs
{
    public class AuthenticationJobs
    {
        public Task SendCodeAsync(SendOTP SendOTPDto)
        {
           BackgroundJob.Enqueue<IAuthenticationBackgroundJob>(job => job.SendCodeAsync(SendOTPDto));
            return Task.CompletedTask;
        }
    }
}
