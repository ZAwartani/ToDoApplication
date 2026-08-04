using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Include.CategoryQ;

namespace TodoApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "UserAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ICategoryService _categoryService;

        public AdminController(IAdminService adminService , ICategoryService categoryService)
        {
            _adminService = adminService;
            _categoryService = categoryService;
        }


        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _adminService.ListOfUserForAdmin();

            if (users == null)
                return NotFound();

            return Ok(users);
        }


        [HttpPost("create-user")]
        public async Task<IActionResult> CreateAdminUser(RegisterNewUserDto newUser)
        {
            var result = await _adminService.AdminCanCreateAdminUser(newUser);

            if (!result)
                return BadRequest("Cannot create user");

            return Ok("User created successfully");
        }


        [HttpDelete("remove-user/{email}")]
        public async Task<IActionResult> RemoveUser(string email)
        {
            var result = await _adminService.RemoveUserAsync(email);

            if (!result)
                return NotFound("User not found");

            return Ok("User removed successfully");
        }


        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(AdminRestPasswordUserDto dto)
        {
            var result = await _adminService.ChangePasswordUserByAdmin(dto);

            if (!result)
                return BadRequest("Password change failed");

            return Ok("Password changed successfully");
        }


        [HttpPut("update-todo/{id}")]
        public async Task<IActionResult> UpdateTodo(UpdateTodoDto dto, int id)
        {
            var result = await _adminService.UpdateTodoForUser(dto, id);

            if (!result)
                return BadRequest("Todo update failed");

            return Ok("Todo updated successfully");
        }

        [HttpGet("GetTodoWithCategory/{UserId}")]
        public async Task<IActionResult> GetTodoWithCategory(int UserId)
        {
            var spec = new IncludeCategoryWithTodosItem(UserId);
            var todo = await _categoryService.ListAsync(spec);

            if (todo == null)
                return NotFound("This User Dont Found Or Don't Have TodoItem");

            return Ok(todo);
        }
    }
}