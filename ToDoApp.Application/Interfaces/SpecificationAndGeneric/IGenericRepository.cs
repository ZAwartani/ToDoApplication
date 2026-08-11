using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.DTOs;
using ToDoApp.Domain.Interfaces;

namespace ToDoApp.Application.Interfaces.Specification
{
    public interface IGenericRepository<TEntity , TKey> where TEntity : IEntity<TKey>
    {
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task UpdateRangeAsync(IReadOnlyList<TEntity> entity);
        Task RemoveRangeAsync(IReadOnlyList<TEntity> entity);
        Task DeleteAsync(TKey Tkey);
        Task<IReadOnlyList<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(TKey key);
        Task<TEntity> GetAsync(ISpecification<TEntity> specification);
        Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification);

    }
}
