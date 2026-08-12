using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ToDoApp.Application.Interfaces.Specification;

namespace ToDoApp.Application.Concreate.Specification
{
    public class BaseSpecificationBuilder<TEntity> : ISpecification<TEntity>
    {
        public Expression<Func<TEntity, bool>>? Criteria { get; set; }

        public List<Expression<Func<TEntity, object>>> Include { get; } = new();

        public Expression<Func<TEntity, object>>? OrderBy { get; set; }

        public Expression<Func<TEntity, object>>? OrderByDescending { get; set; }

        public int Skip { get; set; }

        public int Take {  get; set; }

        public bool IsPagging { get; set; }
       

        private BaseSpecificationBuilder()
        {

        }

        public static ISpecification<TEntity> Create()
        {
            return new BaseSpecificationBuilder<TEntity>();
        }

        public BaseSpecificationBuilder<TEntity> AddCriteria(Expression<Func<TEntity, bool>> criteria)
        {
            Criteria = criteria;
            return this;
        }
        public BaseSpecificationBuilder<TEntity> AddInclude(Expression<Func<TEntity, object>> include)
        {
            Include.Add(include);
            return this;
        }

        public BaseSpecificationBuilder<TEntity> ApplyOrderBy(Expression<Func<TEntity, object>> orderBy)
        {
            this.OrderBy = orderBy;
            return this;

        }
        public BaseSpecificationBuilder<TEntity> ApplyOrderByDesc(Expression<Func<TEntity , object>> orderByDesc)
        {
            this.OrderByDescending = orderByDesc;
            return this;

        }
        public BaseSpecificationBuilder<TEntity> ApplyPaging(
            int pageIndex,
            int pageSize)
        {
            Skip = (pageIndex - 1) * pageSize;
            Take = pageSize;
            IsPagging = true;
            return this;
        }
    }
}
