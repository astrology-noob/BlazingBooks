using BlazingBooks.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazingBooks.Services
{
    public class GenreService
    {
        private AppDBContext _dbContext;

        public GenreService(AppDBContext db)
        {
            _dbContext = db;
        }

        public async Task<List<Genre>> GetGenresFromDBAsync()
        {
            return await Task.Run(() => _dbContext.Genres.Include(g => g.Books).ToList());
        }

        public async Task<Genre> GetGenreByName(string genreName)
        {
            return await Task.Run(() => GetGenresFromDBAsync().Result.FirstOrDefault(g => g.Name == genreName) ?? new Genre());
        }
    }
}
