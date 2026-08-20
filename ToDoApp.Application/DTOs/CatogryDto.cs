using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.DTOs
{
    public record ListCategoryDto(int Id , string TitleCategory);
    public record CategoryAndToDoItemDto(int Id, string TitleCategory , ICollection<TodoItemDTO>? Item);
    public record ReturnCategory(string Title , int UserId);
}
