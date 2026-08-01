using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Searching.Todo
{
    public class SearchingTodoByCategory : BaseSpecification<ToDoItem>
    {
        public SearchingTodoByCategory(int CategoryId) : base(x => x.CategoryId == CategoryId)
        {
            
        }
    }
}
