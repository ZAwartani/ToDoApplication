using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.Dtos;

namespace ToDoApp.Application.Interfaces
{
    public interface IFileUploadService
    {
        Task<FileUploadDTO?> UploadAsync(FileUploadDTO dto);
        Task<IReadOnlyList<string>> ListOfFiles();
        Task<IReadOnlyList<ReturnToClientFileUploadDTO>> GetByCategoryIdAsync(int categoryId);
        Task<IReadOnlyList<ReturnToClientFileUploadDTO>> GetByTodoIdAsync(int todoItemId);
        Task<bool> DeleteAsync(int FileId);
        Task<(byte[] Content, string ContentType, string FileName)?> DownloadAsync(int FileId);
    }
}
