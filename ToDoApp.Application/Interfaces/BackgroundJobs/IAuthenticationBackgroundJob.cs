using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.Dtos;

namespace ToDoApp.Application.Interfaces.BackgroundJob
{
    public interface IAuthenticationBackgroundJob
    {
        public Task SendOTPAsync(SendOTPDto SendOTPDto);
    }
}
