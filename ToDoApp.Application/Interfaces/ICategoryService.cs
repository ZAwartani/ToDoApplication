using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using ToDoApp.Application.Dtos;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces
{
    public interface ICategoryService
    {
        public Task<bool> CreateAsync(UserCategoryDto CategoryDto );
        public Task<bool> UpdateAsync(UserCategoryDto CategoryDto , int Id);
        public Task<bool> DeleteAsync(int Id,bool DeleteWithAllTodos);
        public Task<IReadOnlyList<CategoryInfoDto>> ListAsync();
        public Task<IReadOnlyList<CategoryWithTodosDto>> GetDetailsAsync(int CategoryId);
    }
}
