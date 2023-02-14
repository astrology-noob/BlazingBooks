using BlazingBooks.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazingBooks.Services
{
    public class AuthorService
    {
        private AppDBContext _dbContext;

        public AuthorService(AppDBContext db)
        {
            _dbContext = db;
        }

        public async Task<List<Author>> GetAuthorsFromDBAsync()
        {
            return await Task.Run(() => _dbContext.Authors.Include(g => g.Books).ToList());
        }

        public async Task<Author> GetAuthorByName(string authorName)
        {
            return await Task.Run(() => GetAuthorsFromDBAsync().Result.FirstOrDefault(a => a.Name == authorName) ?? new Author());
        }
    }
}
