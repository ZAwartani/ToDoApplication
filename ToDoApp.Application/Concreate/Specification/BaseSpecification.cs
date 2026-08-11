using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ToDoApp.Application.Interfaces.Specification;

namespace ToDoApp.Application.Concreate.Specification
{
    public class BaseSpecification<TEntity> : ISpecification<TEntity>
    {
        public Expression<Func<TEntity, bool>>? Criteria { get; set; }

        public List<Expression<Func<TEntity, object>>> Include { get; } = new();

        public Expression<Func<TEntity, object>>? OrderBy { get; set; }

        public Expression<Func<TEntity, object>>? OrderByDescending { get; set; }

        public int Skip { get; set; }

        public int Take {  get; set; }

        public bool IsPagging { get; set; }
       

        public BaseSpecification()
        {
        }

        public void AddCriteria(Expression<Func<TEntity, bool>> criteria)
        {
            Criteria = criteria;
        }
        public void AddInclude(Expression<Func<TEntity, object>> include)
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
        public void ApplyPaging(
            int pageIndex,
            int pageSize)
        {
            Skip = (pageIndex - 1) * pageSize;
            Take = pageSize;
            IsPagging = true;
        }
    }
}
