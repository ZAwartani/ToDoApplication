using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ToDoApp.Application.Interfaces.Specification
{
    public class BaseSpecification<TEntity> : ISpecification<TEntity>
    {
        public Expression<Func<TEntity, bool>>? Criteria { get; set; }

        public List<Expression<Func<TEntity, object>>> Include { get; } = new();

        public Expression<Func<TEntity, object>>? OrderBy { get; set; }

        public Expression<Func<TEntity, object>>? OrderByDescending { get; set; }

        public int Skip => throw new NotImplementedException();

        public int Task => throw new NotImplementedException();

        public bool IsPagging => throw new NotImplementedException();
        

        public BaseSpecification(Expression<Func<TEntity, bool>> Criteria)
        {
            this.Criteria = Criteria;
        }
        protected void AddInclude(Expression<Func<TEntity, object>> include)
        {
            Include.Add(include);
        }

        public void ApplyOrderBy(Expression<Func<TEntity, object>> orderBy)
        {
            this.OrderBy = orderBy;
        }
        public void ApplyOrderByDesc(Expression<Func<TEntity , object>> orderByDesc)
        {
            this.OrderByDescending = orderByDesc;
        }
    }
}
