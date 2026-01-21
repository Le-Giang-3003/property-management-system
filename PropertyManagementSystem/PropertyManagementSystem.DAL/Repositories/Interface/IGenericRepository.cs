using System.Linq.Expressions;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdAsync(params object[] keyValues);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, object>>? orderBy = null,
            bool ascending = true,
            params Expression<Func<T, object>>[] includes);

        IQueryable<T> Query();
        IQueryable<T> QueryWithIncludes(params Expression<Func<T, object>>[] includes);
        IQueryable<T> QueryNoTracking();

        Task<bool> AnyAsync();
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        Task<bool> DeleteByIdAsync(int id);

        void SoftDelete(T entity);
        Task<bool> SoftDeleteByIdAsync(int id);

        Task<T?> MaxAsync(Expression<Func<T, bool>>? predicate = null);
        Task<T?> MinAsync(Expression<Func<T, bool>>? predicate = null);
        Task<decimal> SumAsync(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>>? predicate = null);
        Task<double> AverageAsync(Expression<Func<T, int>> selector, Expression<Func<T, bool>>? predicate = null);
    }
}
