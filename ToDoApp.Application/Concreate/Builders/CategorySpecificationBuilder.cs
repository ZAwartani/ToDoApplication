using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using ToDoApp.Application.Concreate.Specification;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Concreate.Builders
{
    public class CategorySpecificationBuilder
    {
        private readonly BaseSpecification<Category> _specification;
        public CategorySpecificationBuilder()
        {
            _specification = new BaseSpecification<Category>();
        }

        public CategorySpecificationBuilder ByUser(int UserId)
        {
            _specification.AddCriteria(
                  c => c.ToDoItems.Any(t => t.UserId == UserId)
             );
            return this;
        }

        public CategorySpecificationBuilder ByCategoryId(int categoryId)
        {
            _specification.AddCriteria(
                    c => c.Id == categoryId
                );
            return this;
        }
        
        public CategorySpecificationBuilder IncludeTodos()
        {
            _specification.AddInclude(c => c.ToDoItems);
            return this;
        }

        public CategorySpecificationBuilder WithTitle(string title)
        {
            _specification.AddCriteria(
                    c => c.TitleOfCategory.Contains(title)
                );
            return this;
        }

        public ISpecification<Category> Build()
        {
            return _specification;
        }
    }
}
