using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Enums;

namespace TodoApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;


        public TodoController(ITodoService todoService )
        {
            this._todoService = todoService;
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateTodoDto dto)
        {
            int UserId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );

            await _todoService.Create(dto, UserId);   
            return Ok("Created Successfully");
        }


        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateTodoDto dto)
        {
            int UserId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );
            var result = await _todoService.Update(
                dto,
                id,
                UserId
            );
            if (!result)
                return BadRequest("Todo not found or not yours");

            return Ok("Updated Successfully");
        }


        


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );

            var result = await _todoService.Delete(
                id,
                userId
            );

            if (!result)
                return NotFound("Todo not found or not yours");

            return Ok("Deleted Successfully");
        }

        [HttpPatch("MarkComplete/{id}")]
        public async Task<IActionResult> MarkComplete(
            int id,
            MarkAsComplete dto)
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );
            var result = await _todoService.MarkAsComplete(
                dto,
                id,
                userId
            );

            if (!result)
                return NotFound("Todo not found or not yours");

            return Ok("Status Updated");
        }

        [HttpGet("GetMyTodos/{FilterBy}/{Descending}")]
        public async Task<IActionResult> GetMyTodos(string FilterBy, bool Descending)
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );

            var todos = await _todoService.ListAsync(userId, FilterBy, Descending);
            return Ok(todos);
        }
    }
}