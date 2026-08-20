using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.Concreate.Specification;
using ToDoApp.Application.Interfaces.Factory;
using ToDoApp.Application.Interfaces.Specification;

namespace ToDoApp.Application.Concreate.Factoryes
{
    public class SpeificationBuilderFactory : ISpecificationBuilderFactory{
        public ISpecification<TEntity> CreateBuilder<TEntity>() where TEntity : class
        {
            return BaseSpecificationBuilder<TEntity>.Create();
        }
    }
}
