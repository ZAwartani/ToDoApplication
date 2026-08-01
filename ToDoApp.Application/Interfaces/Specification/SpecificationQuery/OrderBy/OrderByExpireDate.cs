using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces.Specification.SpecificationQuery.OrderBy
{
    public class OrderByExpireDate : BaseSpecification<ToDoItem>
    {
        public OrderByExpireDate(bool Desc = false) : base(x => true)
        {
            if (Desc)
            {
                ApplyOrderBy(x => x.ExpireDate);
            }
            else
            {
                ApplyOrderByDesc(x => x.ExpireDate);
            }
        }
    }
}
