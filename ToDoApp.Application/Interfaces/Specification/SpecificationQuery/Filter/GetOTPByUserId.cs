using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Filter
{
    public class GetOTPByUserId : BaseSpecification<CodeOTP>
    {
        public GetOTPByUserId(int? CodeId) : base(x => x.Id == CodeId)
        {
            
        }
    }
}
