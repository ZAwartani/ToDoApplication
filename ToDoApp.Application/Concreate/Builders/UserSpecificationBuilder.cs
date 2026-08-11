using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using ToDoApp.Application.Concreate.Specification;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Concreate.Builders
{
    public class UserSpecificationBuilder
    {
        private readonly BaseSpecification<Users> _specification;

        public UserSpecificationBuilder()
        {
            _specification = new BaseSpecification<Users>();
        }

        public UserSpecificationBuilder WithEmail(string email)
        {
            _specification.AddCriteria(
                    user => user.Email.Contains(email)
                );
            return this;
        }

        public BaseSpecification<Users> Build()
        {
            return _specification;
        }
    }
}
