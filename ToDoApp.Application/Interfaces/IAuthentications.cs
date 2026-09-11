using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.Dtos;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces
{
    public interface IAuthentications
    {
        Task RegisterNewUserAsync(RegisterNewUserDto RegisterDto);
        Task<TokenRespone> LoginAsync(LoginDto LoginDto);
        Task LogoutAsync(LogoutDto LogoutDto);
        Task<string?> ChangePasswordAsync(ForgetPasswordOTP ForgetDto);
        Task<TokenRespone> RefreshTokenAsync(RefreshToken RefreshToken);
        Task<IReadOnlyList<UserListDTO>?> ListOfUserForAdminAsync();
        Task<bool> CreateAdminUserAsync(RegisterNewUserDto NewUser);
        Task<bool> RemoveUserAsync(string Email);
        Task<bool> ChangePasswordUserByAdminAsync(AdminRestPasswordUserDto AdminRestPasswordUserDto);

    }
}
