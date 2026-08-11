using System;
using System.Collections.Generic;
using System.Security.Cryptography.Pkcs;
using System.Text;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.DTOs
{
    public record ReturnTodoItem(
    int Id,
    string Title,
    string Description,
    EnumPriority Priority,
    DateTime CreatedAt,
    DateTime ExpireDate,
    string TitleCategory,
    EnumCompleteStatus CompleteStatus,
    EnumRecurring? Recurring
  );


    public record CreateTodoDto(
        string Title,
        string Description,
        EnumPriority Priority,
        DateTime ExpireDate,
        string? TitleCategory,
        EnumRecurring? Recurring
    );


    public record UpdateTodoDto(
        string? Title,
        string? Description,
        EnumPriority? Priority,
        DateTime? ExpireDate,
        string? TitleCategory,
        EnumRecurring? Recurring,
        EnumCompleteStatus? IsComplete
        
    );


    public record MarkAsComplete(
        bool IsComplete
    );
    public record TodoItemDTO
    (
        int Id,
        string Title,
        string Description,
        EnumPriority Priority,
        DateTime ExpireDate,
        EnumCompleteStatus IsComplete
    );

    public record TodoItemSendNotificationDTO
    (
        string Title,
        DateTime ExpireAt
    );
}
