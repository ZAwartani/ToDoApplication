using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Filter
{
    public class GetTokenByIdUser : BaseSpecification<Token>
    {
        public GetTokenByIdUser(int UserId) :base(x => x.UserId == UserId)
        {
            
        }
    }
}
