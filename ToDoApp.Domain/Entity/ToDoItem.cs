using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.Text;
using ToDoApp.Domain.Enums;
using ToDoApp.Domain.Interfaces;

namespace ToDoApp.Domain.Entity
{
    public class ToDoItem : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public EnumPriority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpireDate { get; set; }

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [ForeignKey(nameof(Users))]
        public int UserId { get; set; }
        public Users Users { get; set; }

        public EnumCompleteStatus IsComplete { get; set; }

        //public ICollection<FilesUpload> Files { get; set; }
        
        public EnumRecurring? RecurringSetting { get; set; }

        public ToDoItem(string title,string description,EnumPriority priority,int UserId , DateTime expireDate,int categoryId)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("The Title Is Empty");
            }
            Title = title;
            Description = description;
            Priority = priority;
            this.UserId = UserId;
            this.CreatedAt = DateTime.UtcNow;
            ExpireDate = expireDate;
            CategoryId = categoryId;
            IsComplete = EnumCompleteStatus.NotComplete;

        }

        public ToDoItem(string title , string description , EnumPriority priority , int UserId , DateTime expireDate , int categoryId , int AttachmentId)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("The Title Is Empty");
            }
            Title = title;
            Description = description;
            Priority = priority;
            this.UserId = UserId;
            this.CreatedAt = DateTime.UtcNow;
            ExpireDate = expireDate;
            this.UserId = UserId;
            CategoryId = categoryId;
            IsComplete = EnumCompleteStatus.NotComplete;

        }


        public void Update(
            string? title,
            string? description,
            EnumPriority? priority,
            DateTime? expireDate,
            int categoryId,
            EnumRecurring? recurring)
        {
            if (!string.IsNullOrWhiteSpace(title))
                Title = title;

            if (!string.IsNullOrWhiteSpace(description))
                Description = description;

            if (priority.HasValue)
                Priority = priority.Value;

            if (expireDate.HasValue)
                ExpireDate = expireDate.Value;

            CategoryId = categoryId;

            if (recurring.HasValue)
                RecurringSetting = recurring;
        }
        public void UpdateStatus(bool IsComplete)
        {
            if (IsComplete)
            {
                this.IsComplete = EnumCompleteStatus.Complete;
            }
            else
            {
                this.IsComplete = EnumCompleteStatus.NotComplete;
            }
        }
    }
}
