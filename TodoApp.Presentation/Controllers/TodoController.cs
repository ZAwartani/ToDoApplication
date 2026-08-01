using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Searching.Todo;

namespace TodoApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodoController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateTodoDto dto)
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );


            var result = await _todoService.CreateTodo(
                dto,
                userId
            );


            if (!result)
                return BadRequest("Category not found");


            return Ok("Created Successfully");
        }


        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateTodoDto dto)
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );
            var result = await _todoService.UpdateTodo(
                dto,
                id,
                userId
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

            var result = await _todoService.DeleteTodo(
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
            var result = await _todoService.MarkTodoIsComplete(
                dto,
                id,
                userId
            );

            if (!result)
                return NotFound("Todo not found or not yours");

            return Ok("Status Updated");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyTodos()
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );


            var specification =
                new GetUserTodosSpecification(userId);
            var result =
                await _todoService.ListAsync(specification);

            if (result.Count > 0)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}