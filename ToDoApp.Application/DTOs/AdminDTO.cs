using System;
using System.Collections.Generic;
using System.Text;

namespace ToDoApp.Application.DTOs
{
    public record AdminRestPasswordUserDto(string Email, string Password);
    public record UserListDTO(int Id, string Email);
}
