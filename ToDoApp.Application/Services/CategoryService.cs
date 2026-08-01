using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Searching.CategoryQ;
using ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Searching.Todo;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category, int> _categoryRepository;
        private readonly IGenericRepository<ToDoItem, int> _todoRepository;

        public CategoryService(
            IGenericRepository<Category, int> categoryRepository,
            IGenericRepository<ToDoItem, int> todoRepository)
        {
            _categoryRepository = categoryRepository;
            _todoRepository = todoRepository;
        }

        public async Task<bool> CreateCategory(
            CreateDto categoryDto,
            int userId)
        {
            var spec = new SearchingTitleOnCategory(
                categoryDto.TitleCategory
            );

            var result = await _categoryRepository.ListAsync(spec);

            if (result.Any())
                return false;


            var category = new Category(
                categoryDto.TitleCategory
            );

            await _categoryRepository.AddAsync(category);

            return true;
        }

        public async Task<bool> DeleteCategory(
            int id,
            int userId)
        {
            var category = await _categoryRepository.GetByIdAsync(id);


            if (category == null)
                return false;

            await _categoryRepository.DeleteAsync(id);

            return true;
        }

        public async Task<bool> DeleteCategoryWithAllTodo(
            int id,
            int userId)
        {

            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                return false;
            
            var spec = new SearchingTodoByCategory(id);

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

            if (category == null)
                return false;

            var spec = new SearchingTodoByCategory(id);

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

        public async Task<IReadOnlyList<CategoryAndToDoItemDto>?> ListAsync(
            ISpecification<Category> specification)
        {


            var result = await _categoryRepository.ListAsync(specification);
            if (!result.Any()) return null;

            var mapped = result.Select(c =>
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
            CreateDto categoryDto,
            int id,
            int userId)
        {

            var category = await _categoryRepository
                .GetByIdAsync(id);
            if (category == null)
                return false;

            category.UpdateTitle(
                categoryDto.TitleCategory
            );

            await _categoryRepository.UpdateAsync(category);

            return true;
        }
    }
}