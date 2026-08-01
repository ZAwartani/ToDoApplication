using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.DTOs;

namespace ToDoApp.Application.Interfaces
{
    public interface IAuthentications
    {
        Task RegisterNewUserAsync (RegisterNewUserDto registerDto);
        Task<TokenRespone> Login(LoginDto loginDto);
        Task<bool> Logout(LogoutDto logoutDto);
        Task<string?> ChangePassword(ForgetPasswordOTP ForgetDto);
        Task<bool> SendCodeAsync(SendOTP sendOTPDto);
        Task<TokenRespone> RefreshTokenAsync(RefreshToken refreshToken);

        

    }
}
