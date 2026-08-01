using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Entity;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Filter
{
    public class FilterToDoByStatus:BaseSpecification<ToDoItem>
    {
        public FilterToDoByStatus(EnumCompleteStatus status)
            : base(x => x.IsComplete == status)
        {
        }
    }
}
