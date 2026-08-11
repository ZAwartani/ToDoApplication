using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.Concreate.Specification;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Concreate.Builders
{
    public class FileUploadSpecificationBuilder
    {
        private readonly BaseSpecification<FilesUpload> _specification;

        public FileUploadSpecificationBuilder()
        {
            _specification = new BaseSpecification<FilesUpload>();
        }

        public FileUploadSpecificationBuilder GetFilesByCategoryId(int CategoryId)
        {
            _specification.AddCriteria(x => x.CategoryId == CategoryId);
            return this;
        }
        
        public FileUploadSpecificationBuilder GetFileByTodoId(int TodoId) {
            _specification.AddCriteria(x => x.ToDoItemId == TodoId);
            return this;
        }
        public BaseSpecification<FilesUpload> Build()
        {
            return _specification;
        }
    }
}
