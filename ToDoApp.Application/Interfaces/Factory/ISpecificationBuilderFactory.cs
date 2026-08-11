using ToDoApp.Application.Concreate.Builders;

namespace ToDoApp.Application.Interfaces.Factory;

public interface ISpecificationBuilderFactory
{
    TodoSpecificationBuilder CreateTodoBuilder();

    CategorySpecificationBuilder CreateCategoryBuilder();

    UserSpecificationBuilder CreateUserBuilder();

    FileUploadSpecificationBuilder CreateFileUploadBuilder();

    CodeOTPSpecificationBuilder CreateCodeOTPBuilder();

    TokenSpecificationBuilder CreateTokenBuilder();
}