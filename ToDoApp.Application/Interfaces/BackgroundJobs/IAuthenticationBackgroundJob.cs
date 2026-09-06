using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.DTOs;

namespace ToDoApp.Application.Interfaces.BackgroundJob
{
    public interface IAuthenticationBackgroundJob
    {
        public Task SendCodeAsync(SendOTP SendOTPDto);
    }
}
