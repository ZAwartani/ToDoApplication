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

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<ToDoItem> ToDoItems { get; set; }

        private Category()
        {

        }
        public Category(string Title , int UserId) 
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                throw new ArgumentNullException(nameof(Title));
            }
            this.Title = Title;
            this.UserId = UserId;
        }

        public void UpdateTitle(string titleCategory)
        {
            this.Title = titleCategory;
        }
    }
}
