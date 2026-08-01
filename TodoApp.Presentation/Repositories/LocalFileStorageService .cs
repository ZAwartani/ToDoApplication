using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using ToDoApp.Application.Interfaces;

namespace ToDoApp.Application.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public LocalFileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File is empty");

            const long maxFileSize = 2 * 1024 * 1024;

            if (file.Length > maxFileSize)
                throw new Exception("File size must be 2 MB or less");

            var folder = Path.Combine(
                _environment.WebRootPath,
                "uploads"
            );

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);

            await file.CopyToAsync(stream);

            return fileName;
        }


        public Task<bool> DeleteFileAsync(string fileName)
        {
            var path = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                fileName
            );

            if (File.Exists(path))
                File.Delete(path);

            return Task.FromResult(true);
        }
    }
}
