using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Searching.Todo
{
    public class SearchingByTitleAndDescription : BaseSpecification<ToDoItem>
    {
        public SearchingByTitleAndDescription(string search)
        : base(x =>
                    (x.Title != null && x.Title.Contains(search)) ||
                    (x.Description != null && x.Description.Contains(search)))
        {}
    }
}
