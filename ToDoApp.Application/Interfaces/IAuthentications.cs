using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.DTOs;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces
{
    public interface IAuthentications
    {
        Task RegisterNewUserAsync (RegisterNewUserDto RegisterDto);
        Task<TokenRespone> Login(LoginDto LoginDto);
        Task Logout(LogoutDto LogoutDto);
        Task<string?> ChangePassword(ForgetPasswordOTP ForgetDto);
        Task<bool> SendCodeAsync(SendOTP SendOTPDto);
        Task<TokenRespone> RefreshTokenAsync(RefreshToken RefreshToken);
        Task<IReadOnlyList<UserListDTO>?> ListOfUserForAdmin();
        Task<bool> CreateAdminUser(RegisterNewUserDto NewUser);
        Task<bool> RemoveUserAsync(string Email);
        Task<bool> ChangePasswordUserByAdmin(AdminRestPasswordUserDto AdminRestPasswordUserDto);

    }
}
