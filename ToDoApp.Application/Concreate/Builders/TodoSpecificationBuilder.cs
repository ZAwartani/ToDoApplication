using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.Concreate.Specification;
using ToDoApp.Domain.Entity;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Concreate.Builders
{
    public class TodoSpecificationBuilder
    {
        private readonly BaseSpecification<ToDoItem> _specification;
        public TodoSpecificationBuilder()
        {
            _specification = new BaseSpecification<ToDoItem>();
        }

        public TodoSpecificationBuilder WithCategoryId(int categoryId)
        {
            _specification.AddCriteria(
                    todo => todo.CategoryId == categoryId
                );
            return this;
        }

        public TodoSpecificationBuilder WithTitle(string title)
        {
            _specification.AddCriteria(
                    todo => todo.Title.Contains(title)
                );
            return this;
        }
        public TodoSpecificationBuilder WithDescription(string description)
        {
            _specification.AddCriteria(
                    todo => todo.Description.Contains(description)
                );
            return this;
        }

        public TodoSpecificationBuilder Complete()
        {
            _specification.AddCriteria(
                    todo => (int)todo.IsComplete == 1
                );
            return this;
        }

        public TodoSpecificationBuilder InComplete()
        {
            _specification.AddCriteria(
                    todo => (int)todo.IsComplete == 0
                );
            return this;
        }

        public TodoSpecificationBuilder ExpireDate(DateTime dateTime)
        {
            _specification.AddCriteria(
                    todo => todo.ExpireDate == dateTime
                );
            return this;
        }

        public TodoSpecificationBuilder WithPriority(EnumPriority priority)
        {
            _specification.AddCriteria(
                    todo => todo.Priority == priority
                );
            return this;
        }

        public TodoSpecificationBuilder OrderByExpireDate(bool ascending)
        {
            if (ascending)
            {
                _specification.ApplyOrderBy(todo => todo.ExpireDate);
            }
            else
            {
                _specification.ApplyOrderByDesc(todo => todo.ExpireDate);
            }
            return this;
        }

        public TodoSpecificationBuilder OrderByPriority(bool ascending)
        {
            if (ascending)
            {
                _specification.ApplyOrderBy(todo => todo.Priority);
            }
            else
            {
                _specification.ApplyOrderByDesc(todo => todo.Priority);
            }
            return this;
        }

        public TodoSpecificationBuilder OrderByStatus(bool ascending)
        {
            if (ascending)
            {
                _specification.ApplyOrderBy(todo => todo.IsComplete);
            }
            else
            {
                _specification.ApplyOrderByDesc(todo => todo.IsComplete);
            }
            return this;
        }

        public TodoSpecificationBuilder GetUserTodoItem(int UserId)
        {
            _specification.AddCriteria(
                    todo => todo.UserId == UserId
                );
            return this;
        }

        public TodoSpecificationBuilder SearchingTodoByCategory(int CategoryId)
        {
            _specification.AddCriteria(
                    todo => todo.CategoryId == CategoryId
                );
            return this;
        }

        public BaseSpecification<ToDoItem> Build()
        {
            return _specification;
        }

        internal object WithUserId(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
