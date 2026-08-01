using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Filter
{
    public class FileUploadByTodoSpecification : BaseSpecification<FilesUpload>
    {
        public FileUploadByTodoSpecification(int todoItemId) :base(f => f.ToDoItemId == todoItemId)
        {
            
        }
    }
}
