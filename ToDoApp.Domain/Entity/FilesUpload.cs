using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ToDoApp.Domain.Interfaces;

namespace ToDoApp.Domain.Entity
{
    public class FilesUpload : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string AddressNameFile { get; set; }

        [ForeignKey(nameof(ToDoItem))]
        public int? ToDoItemId { get; set; }
        public ToDoItem? ToDoItem { get; set; }

        [ForeignKey(nameof(Category))]
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
