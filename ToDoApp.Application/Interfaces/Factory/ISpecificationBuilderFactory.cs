using ToDoApp.Application.Concrete.Specification;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Interfaces;

namespace ToDoApp.Application.Interfaces.Factory;

public interface ISpecificationBuilderFactory
{
    ISpecification<TEntity> CreateBuilder<TEntity>()  where TEntity : class;
}