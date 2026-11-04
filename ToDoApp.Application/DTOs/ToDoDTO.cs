using System;
using System.Collections.Generic;
using System.Security.Cryptography.Pkcs;
using System.Text;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Dtos
{
    public record ReturnTodoItemDto(
    int Id,
    string Title,
    string Description,
    EnumPriority Priority,
    DateTime CreatedAt,
    DateTime? ExpireDate,
    int? CategoryId,
    EnumCompleteStatus CompleteStatus,
    EnumRecurring? Recurring
  );


    public record CreateTodoDto(
        string Title,
        string Description,
        EnumPriority Priority,
        DateTime ExpireDate,
        int UserId,
        int? CategoryId,
        EnumRecurring? Recurring,
        EnumCompleteStatus CompleteStatus
    );


    public record UpdateTodoDto(
        int Id,
        string? Title,
        string? Description,
        EnumPriority? Priority,
        DateTime ExpireDate,
        int UserId,
        int? CategoryId,
        EnumRecurring? Recurring,
        EnumCompleteStatus? IsCompleted
    );       


    public record MarkAsCompleteDto(
        int Id,
        int UserId,
        bool IsComplete
    );

    public record TodoItemDto
    (
        int Id,
        string Title,
        string Description,
        EnumPriority Priority,
        DateTime? ExpireDate,
        int UserId,
        EnumCompleteStatus IsComplete
    );
}
