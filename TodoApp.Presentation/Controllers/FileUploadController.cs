using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.Dtos;
using ToDoApp.Application.Interfaces;

namespace TodoApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles ="User")]
public class FileUploadController : ControllerBase
{
    private readonly IFileService _fileUploadService;

    public FileUploadController(IFileService fileUploadService)
    {
        _fileUploadService = fileUploadService;
    }


    [HttpPost("UploadTodo")]
    public async Task<IActionResult> UploadTodo([FromForm] FileUploadTodoItemDto dto)
    {
        var result = await _fileUploadService.UploadTodoItemAsync(dto);

        if (result == null)
            return BadRequest("File upload failed");

        return Ok(result);
    }

    [HttpPost("UploadCategory")]
    public async Task<IActionResult> UploadCategory([FromForm] FileUploadCategoryDto dto)
    {
        var result = await _fileUploadService.UploadCategoryAsync(dto);

        if (result == null)
            return BadRequest("File upload failed");

        return Ok(result);
    }

    [HttpGet("todo/{todoItemId}")]
    public async Task<IActionResult> GetByTodo(int todoItemId)
    {
        var result = await _fileUploadService
            .GetByTodoIdAsync(todoItemId);

        return Ok(result);
    }


    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var result = await _fileUploadService
            .GetByCategoryIdAsync(categoryId);

        return Ok(result);
    }


    [HttpDelete("{fileId}")]
    public async Task<IActionResult> Delete(int fileId)
    {
        var result = await _fileUploadService.DeleteAsync(fileId);

        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpGet("download/{fileId}")]
    public async Task<IActionResult> Download(int fileId)
    {
        var file = await _fileUploadService.DownloadAsync(fileId);

        if (file == null)
            return NotFound("File not found");

        return File(
            file.Content,
            file.ContentType,
            file.FileName
        );
    }
}