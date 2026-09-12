using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToDoApp.Application.Dtos
{
    public record FileUploadTodoItemDto(IFormFile File, int Id);
    public record FileUploadCategoryDto(IFormFile File, int Id);
    public record FileClient(int Id, string AddressNameFile);
    public record DeleteFileDto(int FileId);
    public record DownloadFileDto(byte[] Content, string ContentType, string FileName);
}
   