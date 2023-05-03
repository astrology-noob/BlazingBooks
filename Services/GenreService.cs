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

        public async Task<List<Genre>> GetGenresReadOnly()
        {
            return await Task.Run(() => _dbContext.Genres.AsNoTrackingWithIdentityResolution().Include(g => g.Books).ToList());
        }

        public async Task<List<Genre>> GetGenresFromDBAsync()
        {
            return await Task.Run(() => _dbContext.Genres.Include(g => g.Books).ToList());
        }

        public async Task<List<Genre>> GetGenreNames()
        {
            return await Task.Run(() => _dbContext.Genres.AsNoTracking().Select(g => new Genre { Name = g.Name }).ToList());
        }

        public async Task<Genre> GetGenreByName(string genreName)
        {
            return await Task.Run(() => _dbContext.Genres.FirstOrDefault(g => g.Name == genreName));
        }

        public async Task<Genre> GetGenreById(int id)
        {
            return await Task.Run(() => _dbContext.Genres.AsNoTracking().FirstOrDefault(g => g.Id == id));
        }

        public async Task UpdateGenreAsync(Genre genre)
        {
            _dbContext.Update(genre);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddGenreAsync(Genre genre)
        {
            _dbContext.Add(genre);
            await _dbContext.SaveChangesAsync();
        }

        // придумать как сделать так, чтобы не удалялась книга, если удаляется жанр
        public async Task RemoveGenreAsync(Genre genre)
        {
            _dbContext.Remove(genre);
            await _dbContext.SaveChangesAsync();
        }
    }
}
