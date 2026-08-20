using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToDoApp.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile File);
        Task<IReadOnlyList<string>> ListFilesAsync();
        Task<bool> DeleteFileAsync(string FileName);
        Task<(byte[] Content, string ContentType, string FileName)> DownloadFileAsync(string FileName);
    }
}
