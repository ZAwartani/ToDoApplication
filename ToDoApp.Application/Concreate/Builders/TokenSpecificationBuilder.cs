using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.Concreate.Specification;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Concreate.Builders
{
    public class TokenSpecificationBuilder
    {
        private readonly BaseSpecification<Token> _specification;

        public TokenSpecificationBuilder()
        {
            _specification = new BaseSpecification<Token>();
        }
        public TokenSpecificationBuilder GetTokenById(int UserId)
        {
            _specification.AddCriteria(x => x.Id == UserId);
            return this;
        }
        public TokenSpecificationBuilder GetTokenByIdUser(int UserId)
        {
            _specification.AddCriteria(x => x.UserId == UserId);
            return this;
        }
        public BaseSpecification<Token> Build()
        {
            return _specification;
        }
    }
}
