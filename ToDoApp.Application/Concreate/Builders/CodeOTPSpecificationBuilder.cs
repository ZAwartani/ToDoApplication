using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ToDoApp.Application.Concreate.Specification;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Concreate.Builders
{
    public class CodeOTPSpecificationBuilder
    {
        private readonly BaseSpecification<CodeOTP> _specification;

        public CodeOTPSpecificationBuilder()
        {
            _specification = new BaseSpecification<CodeOTP>();
        }

        public CodeOTPSpecificationBuilder GetOTPByUserId(int UserId)
        {
            _specification.AddCriteria(x => x.UserId == UserId);
            return this;
        }

        public BaseSpecification<CodeOTP> Build()
        {
            return _specification;
        }
    }
}
