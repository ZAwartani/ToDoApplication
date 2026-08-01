using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Include.CategoryQ
{
    public class IncludeCategoryWithTodosItem : BaseSpecification<Category>
    {
        public IncludeCategoryWithTodosItem(int UserId)
            : base(c => c.ToDoItems.Any(t => t.UserId == UserId))
        {
            AddInclude(x => x.ToDoItems);
        }
    }
}
