using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces
{
    public interface ICategoryService
    {
        public Task<bool> CreateCategory(CreateDto categoryDto , int UserId);
        public Task<bool> UpdateCategory(CreateDto categoryDto , int id , int UserId);
        public Task<bool> DeleteCategory(int id , int UserId);
        public Task<bool> DeleteCategoryWithoutTodo(int Id , int UserId);
        public Task<bool> DeleteCategoryWithAllTodo(int Id , int UserId);
        public Task<IReadOnlyList<CategoryAndToDoItemDto>> ListAsync(ISpecification<Category> specification);
    }
}
