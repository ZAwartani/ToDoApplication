using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Dtos
{
    public record CategoryInfoDto(int Id , string TitleCategory);
    public record CategoryWithTodosDto(int Id, string TitleCategory , ICollection<TodoItemDto>? Item);
    public record UserCategoryDto(string Title , int UserId);
}
     