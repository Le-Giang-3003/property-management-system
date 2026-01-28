using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Repositories.Interface;
using System.Linq.Expressions;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // ========== GET SINGLE ==========

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<T?> GetByIdAsync(params object[] keyValues)
        {
            return await _dbSet.FindAsync(keyValues);
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        // ========== GET MULTIPLE ==========

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.Where(predicate);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        // ========== PAGINATION ==========

        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, object>>? orderBy = null,
            bool ascending = true,
            params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            // Includes
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Filter
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            // Total count
            var totalCount = await query.CountAsync();

            // Order
            if (orderBy != null)
            {
                query = ascending
                    ? query.OrderBy(orderBy)
                    : query.OrderByDescending(orderBy);
            }

            // Pagination
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        // ========== QUERY ==========

        public virtual IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }

        public virtual IQueryable<T> QueryWithIncludes(params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        public virtual IQueryable<T> QueryNoTracking()
        {
            return _dbSet.AsNoTracking();
        }

        // ========== COUNT / EXISTS ==========

        public virtual async Task<bool> AnyAsync()
        {
            return await _dbSet.AnyAsync();
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        // ========== ADD ==========

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        // ========== UPDATE ==========

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        // ========== DELETE ==========

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public virtual async Task<bool> DeleteByIdAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            Delete(entity);
            return true;
        }

        // ========== SOFT DELETE ==========

        public virtual void SoftDelete(T entity)
        {
            // Check if entity has IsDeleted property
            var property = typeof(T).GetProperty("IsDeleted");
            if (property != null && property.PropertyType == typeof(bool))
            {
                property.SetValue(entity, true);

                // Also set DeletedAt if exists
                var deletedAtProperty = typeof(T).GetProperty("DeletedAt");
                if (deletedAtProperty != null)
                {
                    deletedAtProperty.SetValue(entity, DateTime.UtcNow);
                }

                Update(entity);
            }
            else
            {
                // Fallback to hard delete
                Delete(entity);
            }
        }

        public virtual async Task<bool> SoftDeleteByIdAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            SoftDelete(entity);
            return true;
        }

        // ========== AGGREGATE ==========

        public virtual async Task<T?> MaxAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
                return await _dbSet.OrderByDescending(e => e).FirstOrDefaultAsync();

            return await _dbSet.Where(predicate).OrderByDescending(e => e).FirstOrDefaultAsync();
        }

        public virtual async Task<T?> MinAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
                return await _dbSet.OrderBy(e => e).FirstOrDefaultAsync();

            return await _dbSet.Where(predicate).OrderBy(e => e).FirstOrDefaultAsync();
        }

        public virtual async Task<decimal> SumAsync(
            Expression<Func<T, decimal>> selector,
            Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
                return await _dbSet.SumAsync(selector);

            return await _dbSet.Where(predicate).SumAsync(selector);
        }

        public virtual async Task<double> AverageAsync(
            Expression<Func<T, int>> selector,
            Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
                return await _dbSet.AverageAsync(selector);

            return await _dbSet.Where(predicate).AverageAsync(selector);
        }
    }
}