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
        private readonly IGenericRepository<User, int> _RepoUser;
        private readonly ISpecificationBuilderFactory _Factory;
        public CategoryService(
            IGenericRepository<Category, int> CategoryRepository,
            IGenericRepository<ToDoItem, int> TodoRepository,
            IGenericRepository<User,int> UserRepository,
            ISpecificationBuilderFactory factory)
        {
            _RepoCategory = CategoryRepository;
            _RepoTodo = TodoRepository;
            _RepoUser = UserRepository;
            _Factory = factory;
        }

        public Task<bool> CascadeDeleteTodos(int Id, int UserId , bool DeleteWithAllTodos)
        {
            if(DeleteWithAllTodos)
            {
                return DeleteWithAllTodo(Id, UserId);
            }
            else
            {
                return DeleteWithoutTodo(Id, UserId);
            }
        }

        public async Task<bool> Create(
            ReturnCategory CategoryDto,
            int UserId)
        {
            var Spec = _Factory.CreateBuilder<User>()
                .AddCriteria(x => x.Id== UserId);

            var Result = await _RepoUser.GetAsync(Spec);

            if (Result != null)
            {
                var NewCategory = new Category(
                     CategoryDto.Title,
                     UserId
                );
                await _RepoCategory.AddAsync(NewCategory);

                return true;
            }
            return false;
        }

        public async Task<IReadOnlyList<CategoryAndToDoItemDto>> GetDetails(int UserId , int CategoryId)
        {
            var Spec = _Factory.CreateBuilder<Category>()
                .AddCriteria(c => c.UserId == UserId)
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

        public async Task<IReadOnlyList<ListCategoryDto>> List(int UserId)
        {
            var Result = await _RepoCategory.ListAsync(
                _Factory.CreateBuilder<Category>()
                .AddCriteria(c => c.UserId == UserId)
                .ApplyOrderBy(c => c.Title)
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
            ReturnCategory CategoryDto,
            int Id,
            int UserId)
        {
            var Spec = _Factory.CreateBuilder<Category>()
                .AddCriteria(c => c.UserId == UserId);

            var Category = await _RepoCategory
                .GetAsync(Spec);
            
            if (Category == null)
                return false;

            Category.UpdateTitle(
                CategoryDto.Title
            );

            await _RepoCategory.UpdateAsync(Category);

            return true;
        }

        private async Task<bool> DeleteWithAllTodo(
            int Id,
            int UserId)
        {
            var SpecCategory = _Factory.CreateBuilder<Category>()
                .AddCriteria(x => x.UserId == UserId)
                .AddCriteria(x => x.Id == Id);

            var Category = await _RepoCategory.GetAsync(SpecCategory);

            if (Category == null)
                throw new Exception("We Have Problem To Remove It");
            
            var Spec = _Factory.CreateBuilder<ToDoItem>()
                .AddCriteria(t => t.UserId == UserId)
                .AddCriteria(t => t.CategoryId == Id);

            var ToDos = await _RepoTodo.ListAsync(Spec);

            if (ToDos.Any())
            {
                await _RepoTodo.UpdateRangeAsync(ToDos);
            }

            await _RepoCategory.DeleteAsync(Id);
            return true;
        }
        private async Task<bool> DeleteWithoutTodo(
            int Id,
            int UserId)
        {
            var SpecCategory = _Factory.CreateBuilder<Category>()
                 .AddCriteria(x => x.UserId == UserId)
                 .AddCriteria(x => x.Id == Id);
            
            var Category = await _RepoCategory.GetAsync(SpecCategory);

            if (Category == null)
                throw new Exception("We Have Problem To Remove It");


            var Spec = _Factory.CreateBuilder<ToDoItem>()
                .AddCriteria(t => t.UserId == UserId)
                .AddCriteria(t => t.CategoryId == Id);

            var ToDos = await _RepoTodo.ListAsync(Spec);

            foreach (var todo in ToDos)
            {
                todo.CategoryId = null;
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
