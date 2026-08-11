using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.Concreate.Builders;
using ToDoApp.Application.Interfaces.Factory;

namespace ToDoApp.Application.Concreate.Factoryes
{
    public class SpeificationBuilderFactory : ISpecificationBuilderFactory
    {
        public CategorySpecificationBuilder CreateCategoryBuilder()
        {
            return new CategorySpecificationBuilder();
        }

        public CodeOTPSpecificationBuilder CreateCodeOTPBuilder()
        {
            return new CodeOTPSpecificationBuilder();
        }

        public FileUploadSpecificationBuilder CreateFileUploadBuilder()
        {
            return new FileUploadSpecificationBuilder();
        }   

        public TodoSpecificationBuilder CreateTodoBuilder()
        {
            return new TodoSpecificationBuilder();
        }

        public UserSpecificationBuilder CreateUserBuilder()
        {
            return new UserSpecificationBuilder();
        }

        public TokenSpecificationBuilder CreateTokenBuilder()
        {
            return new TokenSpecificationBuilder();
        }
    }
}
