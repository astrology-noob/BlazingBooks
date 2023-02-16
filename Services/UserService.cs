using BlazingBooks.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazingBooks.Services
{
    public class UserService
    {
        private AppDBContext _dbContext;

        public UserService(AppDBContext db)
        {
            _dbContext = db;
        }

        public async Task<List<User>> GetUsers()
        {
            return await Task.Run(() => _dbContext.Users.Include(u => u.Roles).Select(u => u).ToList());
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await Task.Run(() => _dbContext.Users.Where(u => u.Username == username).Include(u => u.Roles).Select(u => u).ToList()[0]);
        }
    }
}
