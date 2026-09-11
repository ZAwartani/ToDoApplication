using ToDoApp.Application.Dtos;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.Factory;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IGenericRepository<FilesUpload, int> _RepoFile;
        private readonly IGenericRepository<Category, int> _RepoCategory;
        private readonly IGenericRepository<ToDoItem, int> _RepoTodo;
        private readonly IFileStorageService _FileStorage;
        private readonly ISpecificationBuilderFactory _Factory;
        private readonly ICurrentUser _CurrentUser;

        private int UserId => _CurrentUser.UserId;

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

        public async Task<(byte[] Content, string ContentType, string FileName)?> DownloadAsync(int fileId)
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

        public async Task<IReadOnlyList<ReturnToClientFileUploadDTO>> GetByCategoryIdAsync(int categoryId)
        {
            var specification = _Factory
                .CreateBuilder<FilesUpload>()
                .AddCriteria(f =>
                    f.CategoryId == categoryId &&
                    f.Category.UserId == UserId);

            var files = await _RepoFile.ListAsync(specification);

            return files.Select(file => new ReturnToClientFileUploadDTO(
                file.Id,
                file.Path,
                file.ToDoItemId,
                file.CategoryId
            )).ToList();
        }

        public async Task<IReadOnlyList<ReturnToClientFileUploadDTO>> GetByTodoIdAsync(int todoItemId)
        {
            var specification = _Factory
                .CreateBuilder<FilesUpload>()
                .AddCriteria(f =>
                    f.ToDoItemId == todoItemId &&
                    f.ToDoItem.UserId == UserId);

            var files = await _RepoFile.ListAsync(specification);

            return files.Select(file => new ReturnToClientFileUploadDTO(
                file.Id,
                file.Path,
                file.ToDoItemId,
                file.CategoryId
            )).ToList();
        }

        public async Task<IReadOnlyList<string>> ListOfFiles()
        {
            return await _FileStorage.ListFilesAsync();
        }

        public async Task<FileUploadDTO?> UploadAsync(FileUploadDTO dto)
        {
            if (dto.TodoId.HasValue)
            {
                var todoSpecification = _Factory
                    .CreateBuilder<ToDoItem>()
                    .AddCriteria(t =>
                        t.Id == dto.TodoId.Value &&
                        t.UserId == UserId);

                var todo = await _RepoTodo.GetAsync(todoSpecification);

                if (todo is null)
                    return null;
            }

            if (dto.CategoryId.HasValue)
            {
                var categorySpecification = _Factory
                    .CreateBuilder<Category>()
                    .AddCriteria(c =>
                        c.Id == dto.CategoryId.Value &&
                        c.UserId == UserId);

                var category = await _RepoCategory.GetAsync(categorySpecification);

                if (category is null)
                    return null;
            }

            var fileName = await _FileStorage.SaveFileAsync(dto.File);

            var file = new FilesUpload
            (
                fileName,
                UserId ,
                dto.TodoId,
                dto.CategoryId
            );

            await _RepoFile.AddAsync(file);

            return new FileUploadDTO(
                dto.File,
                file.ToDoItemId,
                file.CategoryId
            );
        }

    }
}