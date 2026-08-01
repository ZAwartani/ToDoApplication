using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces.Specification.SpecificationQuery.OrderBy
{
    public class OrderByStatus : BaseSpecification<ToDoItem>
    {
        public OrderByStatus(bool Desc = false) : base(x => true)
        {
            if (Desc)
            {
                ApplyOrderBy(x => x.IsComplete);
            }
            else
            {
                ApplyOrderByDesc(x => x.IsComplete);
            }
        }
    }
}
