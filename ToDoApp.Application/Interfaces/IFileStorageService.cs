using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToDoApp.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file);
        Task<IReadOnlyList<string>> ListFilesAsync();
        Task<bool> DeleteFileAsync(string fileName);
        Task<(byte[] Content, string ContentType, string FileName)> DownloadFileAsync(string fileName);
    }
}
