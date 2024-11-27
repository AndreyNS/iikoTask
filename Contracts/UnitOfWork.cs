using BackendService.Data;
using iikoTask.Data;
using iikoTask.Interface;
using iikoTask.Models;

namespace iikoTask.Contracts
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IRepository<User> Users { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Users = new Repository<User>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
