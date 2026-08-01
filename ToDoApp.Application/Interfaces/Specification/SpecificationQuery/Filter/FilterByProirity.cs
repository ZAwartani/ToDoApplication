using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Domain.Entity;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Filter
{
    public class FilterByProirity:BaseSpecification<ToDoItem>
    {
        public FilterByProirity(EnumPriority priority) : base(x => x.Priority == priority)
        {
            
        }
    }
}
