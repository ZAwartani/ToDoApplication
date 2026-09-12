using ToDoApp.Application.Concrete.Specification;
using ToDoApp.Application.Interfaces.Factory;
using ToDoApp.Application.Interfaces.Specification;

namespace ToDoApp.Application.Concrete.Factories
{
    public class SpeificationBuilderFactory : ISpecificationBuilderFactory{
        public ISpecification<TEntity> CreateBuilder<TEntity>() where TEntity : class
        {
            return BaseSpecificationBuilder<TEntity>.Create();
        }
    }
}
