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
        public bool IsSend { get; set; }

        private RecurringTodo()
        {
        }

        public RecurringTodo(int todoId, DateTime OccurrenceDate)
        {
            this.TodoId = todoId;
            this.OccurrenceDate = OccurrenceDate;
            this.IsSend = false;
        }

        public void UpdateOccurrenceDate(DateTime newOccurrenceDate)
        {
            OccurrenceDate = newOccurrenceDate;
        }

        public void UpdateSend(bool SendOrnot)
        {
            if(SendOrnot)
                IsSend = SendOrnot;
            else
                throw new ArgumentException("IsSend can only be set to true.");
        }
    }
}
