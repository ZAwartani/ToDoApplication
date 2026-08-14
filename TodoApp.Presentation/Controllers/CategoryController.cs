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
        public CategoryController(ICategoryService _categoryService)
        {
            this._categoryService = _categoryService;
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
            var result = await _categoryService.Create(dto, userId);
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

            var result = await _categoryService.Update
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
            (int id , bool removeTodoItems)
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );

            var result = await _categoryService.CascadeDeleteTodos
                (id, userId , removeTodoItems);
            if (result)
            {
                return Ok("Remove Category Sucessfull");
            }
            else
            {
                return BadRequest();
            }   
        }

        [HttpGet("GetMyCategoryWithTodo")]
        public async Task<IActionResult> GetMyCategoryWithTodo(int CategoryId)
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );


            var result = await _categoryService.GetDetails(userId , CategoryId);

            return Ok(result);
        }

        [HttpGet("ListCategories")]
        public async Task<IActionResult> ListCategories()
        {
            var result = await _categoryService.List();
            return Ok(result);
        }
    }
}
