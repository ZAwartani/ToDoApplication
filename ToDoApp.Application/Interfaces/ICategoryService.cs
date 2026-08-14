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
        public Task<bool> Create(ReturnCategory categoryDto , int UserId);
        public Task<bool> Update(ReturnCategory categoryDto , int id , int UserId);
        public Task<bool> CascadeDeleteTodos(int id, int UserId , bool DeleteWithAllTodos);
        public Task<IReadOnlyList<ListCategoryDto>> List();
        public Task<IReadOnlyList<CategoryAndToDoItemDto>> GetDetails(int UserId , int CategoryId);
    }
}
