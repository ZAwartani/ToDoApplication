using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.Dtos;

namespace ToDoApp.Application.Interfaces
{
    public interface IFileService
    {
        Task<FileUploadCategoryDto?> UploadCategoryAsync(FileUploadCategoryDto dto);
        Task<FileUploadTodoItemDto?> UploadTodoItemAsync(FileUploadTodoItemDto dto);
        Task<IReadOnlyList<string>> ListAsync();
        Task<IReadOnlyList<FileClient>> GetByCategoryIdAsync(int categoryId);
        Task<IReadOnlyList<FileClient>> GetByTodoIdAsync(int todoItemId);
        Task<bool> DeleteAsync(int FileId);
        Task<DownloadFileDto?> DownloadAsync(int FileId);
    }
}
