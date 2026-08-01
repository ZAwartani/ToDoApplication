using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Filter;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IGenericRepository<FilesUpload, int> _repository;
        private readonly IFileStorageService _fileStorage;

        public FileUploadService(
            IGenericRepository<FilesUpload, int> repository,
            IFileStorageService fileStorage)
        {
            _repository = repository;
            _fileStorage = fileStorage;
        }

        public async Task<bool> DeleteAsync(int fileId)
        {
            var file = await _repository.GetByIdAsync(fileId);

            if (file == null)
                return false;

            await _fileStorage.DeleteFileAsync(file.AddressNameFile);

            await _repository.DeleteAsync(fileId);

            return true;
        }

        public async Task<IReadOnlyList<ReturnToClientFileUploadDTO>> GetFilesByCategoryIdAsync(int categoryId)
        {
            var specification = new FileUploadByCategorySpecification(categoryId);

            var files = await _repository.ListAsync(specification);

            return files.Select(file => new ReturnToClientFileUploadDTO(
                file.Id,
                file.AddressNameFile,
                file.ToDoItemId,
                file.CategoryId
            )).ToList();
        }

        public async Task<IReadOnlyList<ReturnToClientFileUploadDTO>> GetFilesByTodoItemIdAsync(int todoItemId)
        {
            var specification = new FileUploadByTodoSpecification(todoItemId);

            var files = await _repository.ListAsync(specification);


            return files.Select(file => new ReturnToClientFileUploadDTO(
               file.Id,
               file.AddressNameFile,
               file.ToDoItemId,
               file.CategoryId
           )).ToList();
        }

        public async Task<FileUploadDTO?> UploadAsync(FileUploadDTO dto)
        {
            var fileName = await _fileStorage.SaveFileAsync(dto.file);

            var file = new FilesUpload
            {
                AddressNameFile = fileName,
                ToDoItemId = dto.TodoId,
                CategoryId = dto.CategoryId
            };

            await _repository.AddAsync(file);

            return new FileUploadDTO(dto.file, file.ToDoItemId, file.CategoryId);
        }
    }
}
