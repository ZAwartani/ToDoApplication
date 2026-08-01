using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Filter
{
    public class FileUploadByCategorySpecification : BaseSpecification<FilesUpload>
    {
        public FileUploadByCategorySpecification(int categoryId) :base(f => f.CategoryId == categoryId)
        {
            
        }
    }
}
