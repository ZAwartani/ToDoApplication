using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Searching.CategoryQ;
using ToDoApp.Domain.Entity;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Services
{
    public class ToDoService : ITodoService
    {
        //private readonly IGenericRepository<FilesUpload ,int> _genericRepositoryAttachment;
        private readonly IGenericRepository<ToDoItem, int> _genericRepositoryTodoItem;
        private readonly IGenericRepository<Category, int> _genericRepositoryCategory;

        public ToDoService(
            IGenericRepository<ToDoItem, int> genericRepositoryTodoItem,
            IGenericRepository<Category, int> genericRepositoryCategory
            )
        {
            this._genericRepositoryTodoItem = genericRepositoryTodoItem;
            this._genericRepositoryCategory = genericRepositoryCategory;
            //this._genericRepositoryAttachment = _genericRepositoryAttachment;
        }

        public async Task<bool> CreateTodo(CreateTodoDto dto, int userId)
        {
            var specCategory = new SearchingTitleOnCategory(dto.TitleCategory);

            var category = await _genericRepositoryCategory
                .ListAsync(specCategory);


            if (!category.Any())
                return false;


            var todo = new ToDoItem(
                dto.Title,
                dto.Description,
                dto.Priority,
                userId,
                dto.ExpireDate,
                category.First().Id
            );

            await _genericRepositoryTodoItem.AddAsync(todo);

            return true;
        }

        public async Task<IReadOnlyList<ReturnTodoItem>> ListAsync(
            ISpecification<ToDoItem> specification)
        {

            var result = await _genericRepositoryTodoItem
                .ListAsync(specification);

            var CategoricalId = result.First().CategoryId;
            var SearchingForTitleCate = await _genericRepositoryCategory.GetByIdAsync(CategoricalId);
            var CategoricalTitle = SearchingForTitleCate.TitleOfCategory;
            var mapped = result.Select(x => new ReturnTodoItem(
                x.Id,
                x.Title,
                x.Description,
                x.Priority,
                x.CreatedAt,
                x.ExpireDate,
                CategoricalTitle,
                x.IsComplete,
                x.RecurringSetting
            )).ToList();


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
            var spec = new SearchingTitleOnCategory(dto.TitleCategory);
            var resultspec = await _genericRepositoryCategory.ListAsync(spec);
            if (!resultspec.Any()) return false;
            var categoricalId = resultspec.First().Id; 
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

        //public async Task<bool> UploadAttachment(UploadAttachmentsDTO Uploadto)
        //{
            
        //    return true;
        //}

        //public Task<bool> RemoveAttachment(RemoveAttachmentsDTO Removedto)
        //{
        //    throw new ArgumentException();
        //}
    }
}
