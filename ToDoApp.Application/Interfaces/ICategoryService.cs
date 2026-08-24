using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using ToDoApp.Application.DTOs;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces
{
    public interface ICategoryService
    {
        public Task<bool> Create(ReturnCategory CategoryDto );
        public Task<bool> Update(ReturnCategory CategoryDto , int Id);
        public Task<bool> CascadeDeleteTodos(int Id,bool DeleteWithAllTodos);
        public Task<IReadOnlyList<ListCategoryDto>> List();
        public Task<IReadOnlyList<CategoryAndToDoItemDto>> GetDetails(int CategoryId);
    }
}
