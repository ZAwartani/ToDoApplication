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


            var result = await _todoService.Create(
                dto,
                userId
            );
            
            BackgroundJob.Schedule(() => _todoService.SendEmailReminderAsync(userId, dto.Title, dto.ExpireDate), dto.ExpireDate.AddDays(-1));

            if(dto.Recurring != null)
            {
                if(dto.Recurring == EnumRecurring.Daily)
                {
                    RecurringJob.AddOrUpdate("daily-recurring", () => _todoService.CreateRecurringTodoAsync(dto , userId, EnumRecurring.Daily), Cron.Daily);
                }
                else if (dto.Recurring == EnumRecurring.Weekly)
                {
                    RecurringJob.AddOrUpdate("weekly-recurring", () => _todoService.CreateRecurringTodoAsync(dto, userId, EnumRecurring.Weekly), Cron.Weekly);
                }
                else if (dto.Recurring == EnumRecurring.Monthly)
                {
                    RecurringJob.AddOrUpdate("monthly-recurring", () => _todoService.CreateRecurringTodoAsync(dto, userId, EnumRecurring.Monthly), Cron.Monthly);
                }
            }    
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
            var result = await _todoService.Update(
                dto,
                id,
                userId
            );

            BackgroundJob.Schedule(() => _todoService.SendEmailReminderAsync(userId, dto.Title, dto.ExpireDate), dto.ExpireDate.AddDays(-1));


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