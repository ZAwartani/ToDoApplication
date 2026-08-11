using System.Net.Http.Headers;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.Factory;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Entity;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Services
{
    public class ToDoService : ITodoService
    {
        private readonly IGenericRepository<ToDoItem, int> _genericRepositoryTodoItem;
        private readonly IGenericRepository<Category, int> _genericRepositoryCategory;

        private readonly ISpecificationBuilderFactory _factory;

        public ToDoService(
            IGenericRepository<ToDoItem, int> genericRepositoryTodoItem,
            IGenericRepository<Category, int> genericRepositoryCategory,
            ISpecificationBuilderFactory factory
            )
        {
            this._genericRepositoryTodoItem = genericRepositoryTodoItem;
            this._genericRepositoryCategory = genericRepositoryCategory;
            this._factory = factory;
        }

        public async Task<bool> CreateTodo(CreateTodoDto dto, int userId)
        {
            var categoryId = 1;
            if(!string.IsNullOrWhiteSpace(dto.TitleCategory)) {
                var specCategory = _factory.CreateCategoryBuilder()
                   .WithTitle(dto.TitleCategory)
                   .Build();

                var category = await _genericRepositoryCategory
                    .GetAsync(specCategory);


                if (category == null)
                    throw new Exception("Category not found");

                else
                    categoryId = category.Id;
            }

            var todo = new ToDoItem(
                dto.Title,
                dto.Description,
                dto.Priority,
                userId,
                dto.ExpireDate,
                categoryId
            );

            await _genericRepositoryTodoItem.AddAsync(todo);

            return true;
        }

        public async Task<IReadOnlyList<ReturnTodoItem>> ListTodoItemAsync( int UserId)
        {
            var spec = _factory.CreateTodoBuilder()
                .GetUserTodoItem(UserId)
                .Build();

            var result = await _genericRepositoryTodoItem.GetAsync(spec);
            if (result == null)
                throw new Exception("User Dont Have Any TodoItem");
            
            var SearchingForTitleCate = await _genericRepositoryCategory.GetByIdAsync(result.CategoryId);
            var CategoricalTitle = SearchingForTitleCate.TitleOfCategory;

            var mapped = new List<ReturnTodoItem>
            {
                new ReturnTodoItem
                (
                    result.Id,
                    result.Title,
                    result.Description,
                    result.Priority,
                    result.CreatedAt,
                    result.ExpireDate,
                    CategoricalTitle,
                    result.IsComplete,
                    result.RecurringSetting
                )
            };


            return mapped;
        }

        public async Task<bool> UpdateTodo(
            UpdateTodoDto dto,
            int id,
            int userId)
        {
            var todo = await _genericRepositoryTodoItem.GetByIdAsync(id);

            if (todo == null || todo.UserId != userId)
                return false;

            var categoricalId = 1;
            if (!string.IsNullOrWhiteSpace(dto.TitleCategory))
            {
                var spec = _factory.CreateCategoryBuilder()
                .WithTitle(dto.TitleCategory)
                .Build();
                var resultspec = await _genericRepositoryCategory.ListAsync(spec);
                if (!resultspec.Any()) return false;
                categoricalId = resultspec.First().Id;
            }
           
            todo.Update(
                dto.Title,
                dto.Description,
                dto.Priority,
                dto.ExpireDate,
                categoricalId,
                dto.Recurring
            );


            await _genericRepositoryTodoItem.UpdateAsync(todo);
            return true;
        }
        public async Task<bool> DeleteTodo(
            int id,
            int userId)
        {

            var todo = await _genericRepositoryTodoItem
                .GetByIdAsync(id);


            if (todo == null)
                return false;

            if (todo.UserId != userId)
                return false;
            await _genericRepositoryTodoItem.DeleteAsync(id);
            return true;
        }
        public async Task<bool> MarkTodoIsComplete(
            MarkAsComplete dto,
            int id,
            int userId)
        {

            var todo = await _genericRepositoryTodoItem
                .GetByIdAsync(id);


            if (todo == null)
                return false;

            if (todo.UserId != userId)
                return false;

            todo.UpdateStatus(dto.IsComplete);

            await _genericRepositoryTodoItem.UpdateAsync(todo);

            return true;
        }

    }
}
