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
            return await Task.Run(() => _dbContext.Authors.FirstOrDefault(a => a.Name == authorName));
        }

        public async Task<Author> GetAuthorById(int id)
        {
            return await Task.Run(() => _dbContext.Authors.FirstOrDefault(a => a.Id == id));
        }

        public async Task<Author> UpdateAuthorAsync(Author author)
        {
            _dbContext.Update(author);
            await _dbContext.SaveChangesAsync();

            return author;
        }

        public async Task<Author> AddAuthorAsync(Author author)
        {
            _dbContext.Add(author);
            await _dbContext.SaveChangesAsync();

            return author;
        }

        public async Task<Author> RemoveAuthorAsync(Author author)
        {
            _dbContext.Remove(author);
            await _dbContext.SaveChangesAsync();

            return author;
        }
    }
}
