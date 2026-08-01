using Microsoft.EntityFrameworkCore;
using TodoApp.Presentation.Persistence;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Interfaces;

namespace TodoApp.Presentation.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity :  IEntity<TKey>
    {
        private readonly AppDbContext _context;
        public GenericRepository(AppDbContext context)
        {
            this._context = context;
        }
        public async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TKey Tkey)
        {
            var entity = await GetByIdAsync(Tkey);
            if(entity != null)
            {
                _context.Set<TEntity>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync()
        {
            var list = await _context.Set<TEntity>().AsNoTracking().ToListAsync();
            return list;
        }

        public async Task<TEntity?> GetByIdAsync(TKey key)
        {
            var res = await _context.Set<TEntity>().FindAsync(key);
            return res;
        }

        public async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification)
        {
            return await SpecificationEva<TEntity>
                .GetQuery(_context.Set<TEntity>().AsQueryable(),specification)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task RemoveRangeAsync(IReadOnlyList<TEntity> entity)
        {
            _context.Set<TEntity>().RemoveRange(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IReadOnlyList<TEntity> entity)
        {
            _context.Set<TEntity>().UpdateRange(entity);
            await _context.SaveChangesAsync();
        }
    }
}
