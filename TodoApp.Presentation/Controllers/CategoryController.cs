using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography.Pkcs;
using ToDoApp.Application.Dtos;
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
        private readonly ICategoryService _CategoryService;

        public CategoryController(ICategoryService _categoryService)
        {
            this._CategoryService = _categoryService;
        }

        [HttpPost("CreateCategory")]
        public async Task<IActionResult> CreateCategory(UserCategoryDto dto)
        {
            var Result = await _CategoryService.CreateAsync(dto);
            if (Result)
            {
                return Ok("Created Sucessful");
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("UpdateCategory/{Id}")]
        public async Task<IActionResult> Update(
            int Id,
            UserCategoryDto dto)
        {
            var Result = await _CategoryService.UpdateAsync
                (dto, Id);
            
            if (Result)
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
            (int Id , bool RemoveTodoItems)
        {
            var result = await _CategoryService.DeleteAsync(Id, RemoveTodoItems);
            return Ok();
        }

        [HttpGet("GetMyCategoryWithTodo")]
        public async Task<IActionResult> GetMyCategoryWithTodo(int CategoryId)
        {
            var Result = await _CategoryService.GetDetailsAsync(CategoryId);

            return Ok(Result);
        }

        [HttpGet("ListCategories")]
        public async Task<IActionResult> ListCategories()
        {
            int UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _CategoryService.ListAsync();
            return Ok(result);
        }
    }
}
