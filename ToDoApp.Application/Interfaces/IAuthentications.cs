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
        Task<TokenResponeDto> LoginAsync(LoginDto LoginDto);
        Task LogoutAsync(LogoutDto LogoutDto);
        Task<string?> ChangePasswordAsync(ForgetPasswordOTPDto ForgetDto);
        Task<TokenResponeDto> RefreshTokenAsync(RefreshTokenDto RefreshToken);
        Task<IReadOnlyList<UserListDto>?> ListUserAsync();
        Task<bool> CreateAdminUserAsync(RegisterNewUserDto NewUser);
        Task<bool> RemoveUserAsync(string Email);
        Task<bool> ChangePasswordUserByAdminAsync(AdminRestPasswordUserDto AdminRestPasswordUserDto);

    }
}
