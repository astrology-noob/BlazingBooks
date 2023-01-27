using BlazingBooks.Data;
using BlazingBooks.Shared;
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

        public async Task<User> GetUserByUsername(string username)
        {
            return await Task.Run(() => (User) _dbContext.Users.Where(u => u.Username == username).Select(u => u));
        }
    }
}
