using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.X509;
using System.Security.Claims;
using ToDoApp.Application.Dtos;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Entity;

namespace TodoApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ICategoryService _CategoryService;
        private readonly IAuthentications _AdminService;
        private readonly ITodoService _TodoService;
        public AdminController( IAuthentications adminService , ICategoryService categoryService, ITodoService todoService)
        {
            _AdminService = adminService;
            _CategoryService = categoryService;
            _TodoService = todoService;
        }


        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _AdminService.ListUserAsync());
        }


        [HttpPost("create-user")]
        public async Task<IActionResult> CreateAdminUser(RegisterNewUserDto newUser)
        {
            var result = await _AdminService.CreateAdminUserAsync(newUser);

            if (!result)
                return BadRequest("Cannot create user");

            return Ok("User created successfully");
        }


        [HttpDelete("remove-user/{email}")]
        public async Task<IActionResult> RemoveUser(string email)
        {
            var result = await _AdminService.RemoveUserAsync(email);

            if (!result)
                return NotFound("User not found");

            return Ok("User removed successfully");
        }


        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(AdminRestPasswordUserDto dto)
        {
            var result = await _AdminService.ChangePasswordUserByAdminAsync(dto);

            if (!result)
                return BadRequest("Password change failed");

            return Ok("Password changed successfully");
        }


        [HttpPut("update-todo/{id}")]
        public async Task<IActionResult> UpdateTodo(UpdateTodoDto dto)
        {

            var result = await _TodoService.UpdateTodoForUserAsync(dto);

            if (!result)
                return BadRequest("Todo update failed");

            return Ok("Todo updated successfully");
        }

        [HttpGet]

        [HttpGet("GetTodoWithCategory/{UserId}/{CategoryId}")]
        public async Task<IActionResult> GetTodoWithCategory(int UserId , int CategoryId)
        {
            var todo = await _CategoryService.GetDetailsAsync(CategoryId);

            if (todo == null)
                return NotFound("This User Dont Found Or Don't Have TodoItem");

            return Ok(todo);
        }
    }
}