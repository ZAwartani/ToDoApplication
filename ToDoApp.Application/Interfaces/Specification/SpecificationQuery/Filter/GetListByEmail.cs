using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Filter
{
    public class GetListByEmail : BaseSpecification<Users>
    {
        public GetListByEmail(string email) : base(x => x.Email == email)
        {

        }
    }
}
