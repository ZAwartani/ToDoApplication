using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.DTOs;

namespace ToDoApp.Application.Interfaces
{
    public interface IFileUploadService
    {
        Task<FileUploadDTO?> UploadAsync(FileUploadDTO dto);
        Task<IReadOnlyList<string>> ListOfFiles();
        Task<IReadOnlyList<ReturnToClientFileUploadDTO>> GetFilesByTodoItemIdAsync(int todoItemId);

        Task<IReadOnlyList<ReturnToClientFileUploadDTO>> GetFilesByCategoryIdAsync(int categoryId);

        Task<bool> DeleteAsync(int fileId);
        Task<(byte[] Content, string ContentType, string FileName)?> DownloadAsync(int fileId);
    }
}
