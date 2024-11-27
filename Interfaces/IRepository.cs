using System.Linq.Expressions;

namespace iikoTask.Interface
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(long id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(long id);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate);
    }
}
