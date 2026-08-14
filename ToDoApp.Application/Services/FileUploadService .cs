using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.Factory;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Services
{
    public class FileUploadService(
        IGenericRepository<FilesUpload, int> _Repository,
        IFileStorageService _FileStorage,
        ISpecificationBuilderFactory specificationBuilderFactory) : IFileUploadService
    {
        public async Task<bool> DeleteAsync(int fileId)
        {
            var file = await _Repository.GetByIdAsync(fileId);

            if (file is null)
                return false;

            await _FileStorage.DeleteFileAsync(file.Path);

            await _Repository.DeleteAsync(fileId);

            return true;
        }

        public async Task<(byte[] Content, string ContentType, string FileName)?> DownloadAsync(int fileId)
        {
            var file = await _Repository.GetByIdAsync(fileId);

            if (file == null)
                return null;

            return await _FileStorage.DownloadFileAsync(file.Path);
        }

        public async Task<IReadOnlyList<ReturnToClientFileUploadDTO>> GetFilesByCategoryIdAsync(int categoryId)
        {
            var specification = specificationBuilderFactory.CreateBuilder<FilesUpload>()
                .AddCriteria(f => f.CategoryId == categoryId);

            var files = await _Repository.ListAsync(specification);

            return files.Select(file => new ReturnToClientFileUploadDTO(
                file.Id,
                file.Path,
                file.ToDoItemId,
                file.CategoryId
            )).ToList();
        }

        public async Task<IReadOnlyList<ReturnToClientFileUploadDTO>> GetFilesByTodoItemIdAsync(int todoItemId)
        {
            var specification = specificationBuilderFactory.CreateBuilder<FilesUpload>()
                .AddCriteria(f => f.ToDoItemId == todoItemId);

            var files = await _Repository.ListAsync(specification);


            return files.Select(file => new ReturnToClientFileUploadDTO(
               file.Id,
               file.Path,
               file.ToDoItemId,
               file.CategoryId
           )).ToList();
        }

        public Task<IReadOnlyList<string>> ListOfFiles()
        {
            var result = _FileStorage.ListFilesAsync();
            return result;
        }

        public async Task<FileUploadDTO?> UploadAsync(FileUploadDTO dto)
        {
            var fileName = await _FileStorage.SaveFileAsync(dto.File);

            var file = new FilesUpload
            {
                Path = fileName,
                ToDoItemId = dto.TodoId,
                CategoryId = dto.CategoryId
            };

            await _Repository.AddAsync(file);

            return new FileUploadDTO(dto.File, file.ToDoItemId, file.CategoryId);
        }
    }
}
