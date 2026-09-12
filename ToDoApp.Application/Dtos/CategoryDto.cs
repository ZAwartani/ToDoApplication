using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Dtos
{
    public record CategoryInfoDto(int Id , string TitleCategory);
    public record UserCategory(int Id, string TitleCategory , ICollection<TodoItemDTO>? Item);
    public record ReturnCategory(string Title , int UserId);
}
     