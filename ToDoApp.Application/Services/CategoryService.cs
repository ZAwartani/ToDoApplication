using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.Factory;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category, int> _RepoCategory;
        private readonly IGenericRepository<ToDoItem, int> _RepoTodo;
        private readonly ISpecificationBuilderFactory _Factory;
        public CategoryService(
            IGenericRepository<Category, int> categoryRepository,
            IGenericRepository<ToDoItem, int> todoRepository,
            ISpecificationBuilderFactory factory)
        {
            _RepoCategory = categoryRepository;
            _RepoTodo = todoRepository;
            _Factory = factory;
        }

        public Task<bool> CascadeDeleteTodos(int id, int UserId , bool DeleteWithAllTodos)
        {
            if(DeleteWithAllTodos)
            {
                return DeleteWithAllTodo(id, UserId);
            }
            else
            {
                return DeleteWithoutTodo(id, UserId);
            }
        }

        public async Task<bool> Create(
            ReturnCategory CategoryDto,
            int userId)
        {
            var NewCategory = new Category(
                 CategoryDto.Title
            );

            await _RepoCategory.AddAsync(NewCategory);

            return true;
        }

        public async Task<IReadOnlyList<CategoryAndToDoItemDto>> GetDetails(int UserId , int CategoryId)
        {
            var Spec = _Factory.CreateBuilder<Category>()
                .AddCriteria(c => c.Id == CategoryId)
                .AddInclude(c => c.ToDoItems);

            var Result = await _RepoCategory.ListAsync(Spec);

            var Mapped = Result.Select(c =>
                new CategoryAndToDoItemDto(
                    c.Id,
                    c.Title,
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

            return Mapped;
        }

        public async Task<IReadOnlyList<ListCategoryDto>> List()
        {
            var Result = await _RepoCategory.ListAsync(
                _Factory.CreateBuilder<Category>().ApplyOrderBy(c => c.Title)
            );

            var Mapped = Result.Select(c =>
                new ListCategoryDto(
                    c.Id,
                    c.Title
                )
            ).ToList();

            return Mapped;
        }


        public async Task<bool> Update(
            ReturnCategory categoryDto,
            int id,
            int userId)
        {

            var Category = await _RepoCategory
                .GetByIdAsync(id);
            
            if (Category == null)
                return false;

            Category.UpdateTitle(
                categoryDto.Title
            );

            await _RepoCategory.UpdateAsync(Category);

            return true;
        }

        private async Task<bool> DeleteWithAllTodo(
            int Id,
            int UserId)
        {

            var Category = await _RepoCategory.GetByIdAsync(Id);

            if (Category == null || Category.Id == 1)
                throw new Exception("We Have Problem To Remove It");

            var spec = _Factory.CreateBuilder<ToDoItem>()
                .AddCriteria(t => t.CategoryId == Id);

            var todos = await _RepoTodo.ListAsync(spec);

            if (todos.Any())
            {
                await _RepoTodo.RemoveRangeAsync(todos);
            }
            await _RepoCategory.DeleteAsync(Id);
            return true;
        }
        private async Task<bool> DeleteWithoutTodo(
            int Id,
            int UserId)
        {

            var Category = await _RepoCategory.GetByIdAsync(Id);

            if (Category == null || Category.Id == 1)
                throw new Exception("We Have Problem To Remove It");


            var Spec = _Factory.CreateBuilder<ToDoItem>()
                .AddCriteria(t => t.CategoryId == Id);

            var ToDos = await _RepoTodo.ListAsync(Spec);

            foreach (var todo in ToDos)
            {
                todo.CategoryId = 1;
            }

            if (ToDos.Any())
            {
                await _RepoTodo.UpdateRangeAsync(ToDos);
            }

            await _RepoCategory.DeleteAsync(Id);

            return true;
        }

    }

}
