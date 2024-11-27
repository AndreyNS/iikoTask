using iikoTask.Data;
using iikoTask.Interface;
using iikoTask.Interfaces;
using iikoTask.Models;

namespace iikoTask.Services
{
    public class UserService : IUserService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _context;
        private readonly ILogger<UserService> _logger;


        public UserService(ILogger<UserService> logger, IUnitOfWork context, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _context = context;
            _logger = logger;
        }

        public async Task<User> Get(long id)
        {
            try
            {
                var user = await _context.Users.GetByIdAsync(id);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{nameof(Get)} Внутренняя ошибка");
                throw;
            }
        }

        public async Task Remove(long id)
        {
            try
            {
                await _context.Users.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{nameof(Remove)} Внутренняя ошибка");
                throw;
            }
        }

        public async Task Update(User user)
        {
            try
            {
                await _context.Users.UpdateAsync(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{nameof(Update)} Внутренняя ошибка");
                throw;
            }
        }

        public async Task<bool> Check(long id) => await _context.Users.AnyAsync(a => a.ClientId == id);

        public async Task<IEnumerable<User>> SetUsers(IEnumerable<User> users)
        {
            var notAdded = new List<User>();

            try
            {
                var tasks = users.Select(async user =>
                {
                    using var scope = _serviceProvider.CreateScope(); 
                    var context = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var exists = await context.Users.AnyAsync(c => c.ClientId == user.ClientId);
                    if (!exists)
                    {
                        await context.Users.AddAsync(user);
                        _logger.LogInformation($"[{nameof(SetUsers)}] Пользователь с {user.ClientId} добавлен");
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        lock (notAdded)
                        {
                            notAdded.Add(user);
                            _logger.LogInformation($"[{nameof(SetUsers)}] Пользователь с {user.ClientId} уже существует");
                        }
                    }
                });

                await Task.WhenAll(tasks);
                _logger.LogInformation($"[{nameof(SetUsers)}] Все операции завершены");

                return notAdded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{nameof(SetUsers)}] Внутренняя ошибка");
                throw;
            }
        }

    }
}
