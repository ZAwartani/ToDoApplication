using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ToDoApp.Application.Interfaces.Specification
{
    public interface ISpecification<TEntity>
    {
        Expression<Func<TEntity, bool>>? Criteria { get; }
        List<Expression<Func<TEntity, object>>> Include { get; }
        Expression<Func<TEntity, object>>? OrderBy { get; }
        Expression<Func<TEntity, object>>? OrderByDescending { get; }
        int Skip { get; }
        int Task { get; }
        bool IsPagging { get; }
    }
}
