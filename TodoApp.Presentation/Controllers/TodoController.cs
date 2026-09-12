using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoApp.Application.Dtos;
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
            await _todoService.CreateAsync(dto);   
            return Ok("Created Successfully");
        }


        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateTodoDto dto)
        {
            var result = await _todoService.UpdateAsync(
                dto
            );
            if (!result)
                return BadRequest("Todo not found or not yours");

            return Ok("Updated Successfully");
        }


        


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            var result = await _todoService.DeleteAsync(
                id
            );

            if (!result)
                return NotFound("Todo not found or not yours");

            return Ok("Deleted Successfully");
        }

        [HttpPatch("MarkComplete/{id}")]
        public async Task<IActionResult> MarkComplete(
            int id,
            MarkAsCompleteDto dto)
        {
            var result = await _todoService.MarkAsCompleteAsync(
                dto
            );

            if (!result)
                return NotFound("Todo not found or not yours");

            return Ok("Status Updated");
        }

        [HttpGet("GetMyTodos/{FilterBy}/{Descending}")]
        public async Task<IActionResult> GetMyTodos(string FilterBy, bool Descending)
        {
            var todos = await _todoService.ListAsync(FilterBy, Descending);
            return Ok(todos);
        }
    }
}