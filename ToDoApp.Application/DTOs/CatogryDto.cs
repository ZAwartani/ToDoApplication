using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.DTOs
{
    public record ListCategoryDto(int id , string TitleCategory);
    public record CategoryAndToDoItemDto(int id, string TitleCategory , ICollection<TodoItemDTO>? Item);
    public record ReturnCategory(string Title);
}
