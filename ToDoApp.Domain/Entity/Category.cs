using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ToDoApp.Domain.Interfaces;

namespace ToDoApp.Domain.Entity
{
    public class Category:IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public ICollection<ToDoItem> ToDoItems { get; set; }

        private Category()
        {

        }
        public Category(string Title   ) 
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                throw new ArgumentNullException(nameof(Title));
            }
            this.Title = Title;
        }

        public void UpdateTitle(string titleCategory)
        {
            this.Title = titleCategory;
        }
    }
}
