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
        Task<IReadOnlyList<ReturnToClientFileUploadDTO>> GetFilesByTodoItemIdAsync(int ToDoItemId);

        Task<IReadOnlyList<ReturnToClientFileUploadDTO>> GetFilesByCategoryIdAsync(int CategoryId);

        Task<bool> DeleteAsync(int FileId);
        Task<(byte[] Content, string ContentType, string FileName)?> DownloadAsync(int FileId);
    }
}
