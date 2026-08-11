using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography.Pkcs;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Services;
using ToDoApp.Domain.Enums;

namespace TodoApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")] 
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ITodoService todoService;
        public CategoryController(ICategoryService _categoryService , ITodoService service)
        {
            this._categoryService = _categoryService;
            this.todoService = service;
        }

        [HttpGet("CheckClaims")]
        public IActionResult CheckClaims()
        {
            var claims = User.Claims.Select(c => new
            {
                c.Type,
                c.Value
            });

            return Ok(claims);
        }

        [HttpPost("CreateCategory")]
        public async Task<IActionResult> CreateCategory(ReturnCategory dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _categoryService.CreateCategory(dto, userId);
            if (result)
            {
                return Ok("Created Sucessful");
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("UpdateCategory/{id}")]
        public async Task<IActionResult> Update(
            int id,
            ReturnCategory dto)
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );

            var result = await _categoryService.UpdateCategory
                (dto, id, userId);
            
            if (result)
            {
                return Ok("Update Sucessfull");
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> Remove
            (int id)
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );

            var result = await _categoryService.DeleteCategory
                (id, userId);
            if (result)
            {
                return Ok("Remove Category Sucessfull");
            }
            else
            {
                return BadRequest();
            }   
        }

        [HttpDelete("DeleteCategoryWithoutRemoveToDoItem/{id}")]
        public async Task<IActionResult> RemoveCategoryWihtoutRemoveToDoItem
            (int id) {
            
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );
            var result = await _categoryService.DeleteCategoryWithoutTodo(id , userId);
            if (result)
            {
                return Ok("Delete Category Without TodoItem");
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete("DeleteCategoryWithRemoveTodoItem/{id}")]
        public async Task<IActionResult> RemoveCategoryWithRemoveTodoItem
            (int id)
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );
            var result = await _categoryService.DeleteCategoryWithAllTodo(id, userId);
            if (result)
            {
                return Ok("Delete Category Without TodoItem");
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("GetMyCategoryWithTodo")]
        public async Task<IActionResult> GetMyCategoryWithTodo()
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );


            var result = await _categoryService.ListCategoryWithTodo(userId);

            return Ok(result);
        }
        [HttpGet("ListCategories")]
        public async Task<IActionResult> ListCategories()
        {
            var result = await _categoryService.ListCategories();
            return Ok(result);
        }
    }
}
