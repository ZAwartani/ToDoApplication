using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.Factory;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category, int> _categoryRepository;
        private readonly IGenericRepository<ToDoItem, int> _todoRepository;
        private readonly ISpecificationBuilderFactory _factory;

        public CategoryService(
            IGenericRepository<Category, int> categoryRepository,
            IGenericRepository<ToDoItem, int> todoRepository,
            ISpecificationBuilderFactory factory)
        {
            _categoryRepository = categoryRepository;
            _todoRepository = todoRepository;
            this._factory = factory;
        }

        public async Task<bool> CreateCategory(
            ReturnCategory categoryDto,
            int userId)
        {

            var spec = _factory.CreateCategoryBuilder()
                .WithTitle(categoryDto.Title)
                .Build();

            var result = await _categoryRepository.ListAsync(spec);

            if (result.Any())
                return false;


            var category = new Category(
                 categoryDto.Title
            );

            await _categoryRepository.AddAsync(category);

            return true;
        }

        public async Task<bool> DeleteCategory(
            int id,
            int userId)
        {
            var category = await _categoryRepository.GetByIdAsync(id);


            if (category == null || category.Id == 1)
                throw new Exception("We Have Problem To Remove It");

            await _categoryRepository.DeleteAsync(id);

            return true;
        }

        public async Task<bool> DeleteCategoryWithAllTodo(
            int id,
            int userId)
        {

            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null || category.Id == 1)
                throw new Exception("We Have Problem To Remove It");

            var spec = _factory.CreateTodoBuilder()
                .WithCategoryId(id)
                .Build();

            var todos = await _todoRepository.ListAsync(spec);

            if (todos.Any())
            {
                await _todoRepository.RemoveRangeAsync(todos);
            }
            await _categoryRepository.DeleteAsync(id);
            return true;
        }
        public async Task<bool> DeleteCategoryWithoutTodo(
            int id,
            int userId)
        {

            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null || category.Id == 1)
                throw new Exception("We Have Problem To Remove It");


            var spec = _factory.CreateTodoBuilder()
                .WithCategoryId(id)
                .Build();

            var todos = await _todoRepository.ListAsync(spec);

            foreach (var todo in todos)
            {
                todo.CategoryId = 1;
            }

            if (todos.Any())
            {
                await _todoRepository.UpdateRangeAsync(todos);
            }

            await _categoryRepository.DeleteAsync(id);

            return true;
        }

        public Task<IReadOnlyList<ListCategoryDto>> ListCategories()
        {
            var result = _categoryRepository.ListAsync(
                _factory.CreateCategoryBuilder()
                .Build()
            );
            var mapped = result.Result.Select(c =>
                new ListCategoryDto(
                    c.Id,
                    c.TitleOfCategory
                )
            ).ToList();
            return Task.FromResult<IReadOnlyList<ListCategoryDto>>(mapped);
        }

        public async Task<IReadOnlyList<CategoryAndToDoItemDto>> ListCategoryWithTodo(int UserId)
        {
            var result = _categoryRepository.ListAsync(
                _factory.CreateCategoryBuilder()
                .ByUser(UserId)
                .IncludeTodos()
                .Build()
            );

            var mapped = (await result).Select(c =>
                new CategoryAndToDoItemDto(
                    c.Id,
                    c.TitleOfCategory,
                    c.ToDoItems.Select(t => new TodoItemDTO(
                        t.Id,
                        t.Title,
                        t.Description,
                        t.Priority,
                        t.ExpireDate,
                        t.IsComplete
                    )).ToList()
                )
            ).ToList();

            return mapped;
        }

        public async Task<bool> UpdateCategory(
            ReturnCategory categoryDto,
            int id,
            int userId)
        {

            var category = await _categoryRepository
                .GetByIdAsync(id);
            if (category == null)
                return false;

            category.UpdateTitle(
                categoryDto.Title
            );

            await _categoryRepository.UpdateAsync(category);

            return true;
        }
    }
}