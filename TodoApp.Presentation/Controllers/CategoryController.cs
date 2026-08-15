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
        private readonly ICategoryService _CategoryService;
        public CategoryController(ICategoryService _categoryService)
        {
            this._CategoryService = _categoryService;
        }

        [HttpGet("CheckClaims")]
        public IActionResult CheckClaims()
        {
            var Claims = User.Claims.Select(c => new
            {
                c.Type,
                c.Value
            });

            return Ok(Claims);
        }

        [HttpPost("CreateCategory")]
        public async Task<IActionResult> CreateCategory(ReturnCategory dto)
        {
            var UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var Result = await _CategoryService.Create(dto, UserId);
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
            ReturnCategory dto)
        {
            int UserId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );

            var Result = await _CategoryService.Update
                (dto, Id, UserId);
            
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
            int UserId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );

            var result = await _CategoryService.CascadeDeleteTodos
                (Id, UserId , RemoveTodoItems);
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
            int UserId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );


            var Result = await _CategoryService.GetDetails(UserId , CategoryId);

            return Ok(Result);
        }

        [HttpGet("ListCategories")]
        public async Task<IActionResult> ListCategories()
        {
            int UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _CategoryService.List(UserId);
            return Ok(result);
        }
    }
}
