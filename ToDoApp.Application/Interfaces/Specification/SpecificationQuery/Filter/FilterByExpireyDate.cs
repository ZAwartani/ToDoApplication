using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Filter
{
    public class FilterByExpireyDate : BaseSpecification<ToDoItem>
    {
        public FilterByExpireyDate(DateTime date) : base(x => x.ExpireDate == date)
        {
            
        }
    }
}
