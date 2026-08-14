using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToDoApp.Application.DTOs
{
    public record FileUploadDTO(IFormFile File, int? TodoId, int? CategoryId);
    public record ReturnToClientFileUploadDTO(int Id, string AddressNameFile, int? TodoId, int? CategoryId);
    public record DeleteFileDTO(int FileId);
}
