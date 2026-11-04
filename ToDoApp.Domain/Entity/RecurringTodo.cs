using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ToDoApp.Domain.Interfaces;

namespace ToDoApp.Domain.Entity
{
    public class RecurringTodo : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Todo))]
        public int TodoId { get; set; }
        public ToDoItem Todo { get; set; }
        public DateTime OccurrenceDate { get; set; }

        public RecurringTodo(int todoId, DateTime OccurrenceDate)
        {
            this.TodoId = todoId;
            this.OccurrenceDate = OccurrenceDate;
        }

        public void UpdateOccurrenceDate(DateTime newOccurrenceDate)
        {
            OccurrenceDate = newOccurrenceDate;
        }
    }
}
