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
        public Task<bool> Create(ReturnCategory CategoryDto , int UserId);
        public Task<bool> Update(ReturnCategory CategoryDto , int Id , int UserId);
        public Task<bool> CascadeDeleteTodos(int Id, int UserId , bool DeleteWithAllTodos);
        public Task<IReadOnlyList<ListCategoryDto>> List(int UserId);
        public Task<IReadOnlyList<CategoryAndToDoItemDto>> GetDetails(int UserId , int CategoryId);
    }
}
