using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Searching.CategoryQ
{
    public class SearchingTitleOnCategory : BaseSpecification<Category>
    {
        public SearchingTitleOnCategory(string title) : base(x => x.TitleOfCategory == title)
        {
            
        }
    }
}
