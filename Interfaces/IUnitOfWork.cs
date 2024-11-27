using iikoTask.Models;

namespace iikoTask.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        Task<int> SaveChangesAsync();
    }
}
