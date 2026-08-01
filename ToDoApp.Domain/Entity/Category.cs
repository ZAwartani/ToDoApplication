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

        public string TitleOfCategory { get; set; }

        //public ICollection<FilesUpload> Files {  get; set; }
        public ICollection<ToDoItem> ToDoItems { get; set; }

        public Category(string TitleOfCategory) 
        {
            if (string.IsNullOrWhiteSpace(TitleOfCategory))
            {
                throw new ArgumentNullException(nameof(TitleOfCategory));
            }
            this.TitleOfCategory = TitleOfCategory;
        }

        public void UpdateTitle(string titleCategory)
        {
            this.TitleOfCategory = titleCategory;
        }
    }
}
