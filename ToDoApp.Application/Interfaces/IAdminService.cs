using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.DTOs;

namespace ToDoApp.Application.Interfaces
{
    public interface IAdminService
    {
        Task<IReadOnlyList<UserListDTO>?> ListOfUserForAdmin();
        Task<bool> AdminCanCreateAdminUser(RegisterNewUserDto NewUser);
        Task<bool> RemoveUserAsync(string Email);
        Task<bool> ChangePasswordUserByAdmin(AdminRestPasswordUserDto adminRestPasswordUserDto);
        Task<bool> UpdateTodoForUser(UpdateTodoDto updateTodoDto, int Id);
    }
}
