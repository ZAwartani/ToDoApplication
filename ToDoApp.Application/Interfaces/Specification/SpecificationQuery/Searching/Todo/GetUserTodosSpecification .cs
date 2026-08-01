using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Searching.Todo
{
    public class GetUserTodosSpecification : BaseSpecification<ToDoItem>
    {
        public GetUserTodosSpecification(int userId) : base(x => x.Id == userId)
        {
        }
    }
}
