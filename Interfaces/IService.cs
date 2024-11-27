using iikoTask.Models;

namespace iikoTask.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> SetUsers(IEnumerable<User> users);
        Task<bool> Check(long id);
        Task<User> Get(long id);
        Task Update(User user);
        Task Remove(long id);
    }
}
