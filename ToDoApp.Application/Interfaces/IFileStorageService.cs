using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.Dtos;

namespace ToDoApp.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile File);
        Task<IReadOnlyList<string>> ListFilesAsync();
        Task<bool> DeleteFileAsync(string FileName);
        Task<DownloadFileDto> DownloadFileAsync(string FileName);
    }
}
