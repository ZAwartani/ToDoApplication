using System;
using System.Collections.Generic;
using System.Text;

namespace ToDoApp.Application.Dtos
{
    public record AdminRestPasswordUserDto(string Email, string Password);
    public record UserListDto(int Id, string Email);
}
  