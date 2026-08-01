using Org.BouncyCastle.Crypto.Engines;
using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces.Specification.SpecificationQuery.OrderBy
{
    public class OrderByPriority : BaseSpecification<ToDoItem>
    {
        public OrderByPriority(bool Desc = false) : base(x => true)
        {
            if (Desc)
            {
                ApplyOrderBy(x => x.Priority);
            }
            else
            {
                ApplyOrderByDesc(x => x.Priority);
            }
        }
    }
}
