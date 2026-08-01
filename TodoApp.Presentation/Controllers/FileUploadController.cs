using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;

namespace TodoApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles ="User")]
public class FileUploadController : ControllerBase
{
    private readonly IFileUploadService _fileUploadService;

    public FileUploadController(IFileUploadService fileUploadService)
    {
        _fileUploadService = fileUploadService;
    }


    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] FileUploadDTO dto)
    {
        var result = await _fileUploadService.UploadAsync(dto);

        if (result == null)
            return BadRequest("File upload failed");

        return Ok(result);
    }


    [HttpGet("todo/{todoItemId}")]
    public async Task<IActionResult> GetByTodo(int todoItemId)
    {
        var result = await _fileUploadService
            .GetFilesByTodoItemIdAsync(todoItemId);

        return Ok(result);
    }


    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var result = await _fileUploadService
            .GetFilesByCategoryIdAsync(categoryId);

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
}