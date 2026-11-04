using ToDoApp.Application.Dtos;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.Factory;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Services
{
    public class FileUploadService : IFileService
    {
        private readonly IGenericRepository<FilesUpload, int> _RepoFile;
        private readonly IGenericRepository<Category, int> _RepoCategory;
        private readonly IGenericRepository<ToDoItem, int> _RepoTodo;
        private readonly IFileStorageService _FileStorage;
        private readonly ISpecificationBuilderFactory _Factory;
        private readonly ICurrentUser _CurrentUser;

        private int UserId => _CurrentUser.Id;

        public FileUploadService(
            IGenericRepository<FilesUpload, int> RepoFile,
            IFileStorageService fileStorageService,
            ISpecificationBuilderFactory specificationBuilderFactory,
            IGenericRepository<Category,int>  RepoCategory,
            IGenericRepository<ToDoItem,int> RepoTodo,
            ICurrentUser currentUser)
        {
            _RepoTodo = RepoTodo;
            _RepoCategory = RepoCategory;
            _CurrentUser = currentUser;
            _RepoFile = RepoFile;
            _FileStorage = fileStorageService;
            _Factory = specificationBuilderFactory;
        }

        public async Task<bool> DeleteAsync(int fileId)
        {
            var specification = _Factory
                .CreateBuilder<FilesUpload>()
                .AddCriteria(f =>
                    f.Id == fileId &&
                    (
                        (f.ToDoItemId != null &&
                         f.ToDoItem.UserId == UserId)
                        ||
                        (f.CategoryId != null &&
                         f.Category.UserId == UserId)
                    ));

            var file = await _RepoFile.GetAsync(specification);

            if (file is null)
                return false;

            await _FileStorage.DeleteFileAsync(file.Path);

            await _RepoFile.DeleteAsync(fileId);

            return true;
        }

        public async Task<DownloadFileDto?> DownloadAsync(int fileId)
        {
            var specification = _Factory
                .CreateBuilder<FilesUpload>()
                .AddCriteria(f =>
                    f.Id == fileId &&
                    (
                        (f.ToDoItemId != null &&
                         f.ToDoItem.UserId == UserId)
                        ||
                        (f.CategoryId != null &&
                         f.Category.UserId == UserId)
                    ));

            var file = await _RepoFile.GetAsync(specification);

            if (file is null)
                return null;

            return await _FileStorage.DownloadFileAsync(file.Path);
        }

        public async Task<IReadOnlyList<FileClient>> GetByCategoryIdAsync(int categoryId)
        {
            var specification = _Factory
                .CreateBuilder<FilesUpload>()
                .AddCriteria(f =>
                    f.CategoryId == categoryId &&
                    f.Category.UserId == UserId);

            var files = await _RepoFile.ListAsync(specification);

            return files.Select(file => new FileClient(
                file.Id,
                file.Path
            )).ToList();
        }

        public async Task<IReadOnlyList<FileClient>> GetByTodoIdAsync(int todoItemId)
        {
            var specification = _Factory
                .CreateBuilder<FilesUpload>()
                .AddCriteria(f =>
                    f.ToDoItemId == todoItemId &&
                    f.ToDoItem.UserId == UserId);

            var files = await _RepoFile.ListAsync(specification);

            return files.Select(file => new FileClient(
                file.Id,
                file.Path
            )).ToList();
        }

        public async Task<IReadOnlyList<string>> ListAsync()
        {
            return await _FileStorage.ListFilesAsync();
        }

        public async Task<FileUploadCategoryDto?> UploadCategoryAsync(FileUploadCategoryDto dto)
        {

                var categorySpecification = _Factory
                    .CreateBuilder<Category>()
                    .AddCriteria(c =>
                        c.Id == dto.Id &&
                        c.UserId == UserId);

                var category = await _RepoCategory.GetAsync(categorySpecification);

                if (category is null)
                    return null;

            var fileName = await _FileStorage.SaveFileAsync(dto.File);

            var file = new FilesUpload
            (
                fileName,
                UserId ,
                dto.Id,
                null
            );

            await _RepoFile.AddAsync(file);

            return new FileUploadCategoryDto(
                dto.File,
                file.CategoryId.Value
            );
        }

        public async Task<FileUploadTodoItemDto?> UploadTodoItemAsync(FileUploadTodoItemDto dto)
        {
            var todoSpecification = _Factory
                .CreateBuilder<ToDoItem>()
                .AddCriteria(t =>
                    t.Id == dto.Id &&
                    t.UserId == UserId);
            var todoItem = _RepoTodo.GetAsync(todoSpecification).Result;
            if (todoItem is null)
                return null;

            var fileName = _FileStorage.SaveFileAsync(dto.File).Result;
            var file = new FilesUpload
            (
                fileName,
                UserId,
                null,
                dto.Id
            );
            await _RepoFile.AddAsync(file);
           return new FileUploadTodoItemDto(
                dto.File,
                file.ToDoItemId.Value
            );
        }
    }
}