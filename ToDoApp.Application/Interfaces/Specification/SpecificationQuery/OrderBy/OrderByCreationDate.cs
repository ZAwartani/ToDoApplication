using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces.Specification.SpecificationQuery.OrderBy
{
    public class OrderByCreationDate : BaseSpecification<ToDoItem>
    {
        public OrderByCreationDate(bool Desc = false)
     : base(x => true)
        {
            if (Desc)
                ApplyOrderByDesc(x => x.CreatedAt);
            else
                ApplyOrderBy(x => x.CreatedAt);
        }
    }
}
