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
        public Task<bool> CreateCategory(ReturnCategory categoryDto , int UserId);
        public Task<bool> UpdateCategory(ReturnCategory categoryDto , int id , int UserId);
        public Task<bool> DeleteCategory(int id , int UserId);
        public Task<bool> DeleteCategoryWithoutTodo(int Id , int UserId);
        public Task<bool> DeleteCategoryWithAllTodo(int Id , int UserId);
        public Task<IReadOnlyList<ListCategoryDto>> ListCategories();
        public Task<IReadOnlyList<CategoryAndToDoItemDto>> ListCategoryWithTodo(int UserId);
    }
}
