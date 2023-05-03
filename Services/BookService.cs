using BlazingBooks.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazingBooks.Services
{
    public class BookService
    {
        private AppDBContext _dbContext;

        public BookService(AppDBContext db)
        {
            _dbContext = db;
        }

        public async Task<List<Book>> GetBooksReadOnly()
        {
            return await Task.Run(() => _dbContext.Books
                                        .AsNoTrackingWithIdentityResolution()
                                        .Include(b => b.Authors).Include(b => b.Genres).ToList());
        }

        public async Task<List<Book>> GetBooksFromDBAsync()
        {
            return await Task.Run(() => _dbContext.Books.Include(b => b.Authors).Include(b => b.Genres).ToList());
        }

        public async Task<Book> GetBookById(int id)
        {
            // было
            //return await _dbContext.Books.FindAsync(id);
            // стало
            return await _dbContext.Books.Include(b => b.Authors).Include(b => b.Genres).FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Book>> GetBooksByGenre(Genre genre)
        {
            return await Task.Run(() => _dbContext.Books
                                        .AsNoTrackingWithIdentityResolution()
                                        .Include(b => b.Authors).Include(b => b.Genres)
                                        .Where(b => b.Genres.Where(g => g.Id == genre.Id).Count() > 0).ToList());
        }

        public async Task<List<Book>> GetBooksByAuthor(Author author)
        {
            return await Task.Run(() => _dbContext.Books
                                        .AsNoTrackingWithIdentityResolution()
                                        .Include(b => b.Authors).Include(b => b.Genres)
                                        .Where(b => b.Authors.Where(a => a.Id == author.Id).Count() > 0).ToList());
        }

        public async Task<List<Book>> GetBooksOrderedByAsync(List<Book> books, PropertyEnum option)
        {
            return await Task.Run(() => option switch
            {
                PropertyEnum.Author => books.OrderBy(book => book.Authors).ToList(),
                PropertyEnum.Title => books.OrderBy(book => book.Title).ToList(),
                PropertyEnum.Published => books.OrderBy(book => book.Published).ToList(),
                PropertyEnum.Price => books.OrderBy(book => book.Price).ToList(),
                _ => throw new NotImplementedException()
            });
        }

        //public async Task<List<Book>> GetBooksByPropertyAsync(List<Book> books, PropertyEnum option, object desiredValue)
        //{
        //    return await Task.Run(() => option switch
        //    {
        //        //PropertyEnum.Author => books.Where(book => book.Authors.Where(a => a.Name.Contains((string)desiredValue)).Select(book => book).ToList(),
        //        PropertyEnum.Title => books.Where(book => book.Title.Contains((string)desiredValue)).Select(book => book).ToList(),
        //        PropertyEnum.Published => books.Where(book => book.Published == (DateTime)desiredValue).Select(book => book).ToList(),
        //        PropertyEnum.Price => books.Where(book => book.Price == (int)desiredValue).Select(book => book).ToList(),
        //        _ => throw new NotImplementedException()
        //    } ?? new List<Book>());
        //}

        public async Task<bool> BuyBooks(KeyValuePair<Book, int> bookCount)
        {
            Book bookToAlter = _dbContext.Books.Find(bookCount.Key.Id);
            if (bookToAlter.TotalCount < bookCount.Value)
            {
                return false;
            }
            bookToAlter.TotalCount -= bookCount.Value;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task UpdateBookAsync(Book book)
        {
            _dbContext.Update(book);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddBookAsync(Book book)
        {
            _dbContext.Update(book);
            _dbContext.ChangeTracker.DetectChanges();
            Console.WriteLine(_dbContext.ChangeTracker.DebugView.LongView);
            await _dbContext.SaveChangesAsync();
            Console.WriteLine(_dbContext.ChangeTracker.DebugView.LongView);
        }

        public async Task RemoveBookAsync(Book book)
        {
            _dbContext.Remove(book);
            _dbContext.ChangeTracker.DetectChanges();
            Console.WriteLine(_dbContext.ChangeTracker.DebugView.LongView);
            await _dbContext.SaveChangesAsync();
            // по какой-то причине после удаления книги начинают трэкаться все связанные объекты базы данных.
            // Чтобы это не мешало дальнейшим изменениям, ChangeTracker очищается, хотя наверняка есть более правильное решение
            _dbContext.ChangeTracker.Clear();
            Console.WriteLine(_dbContext.ChangeTracker.DebugView.LongView);
        }

        public async Task AddGenresToBook(Book book, List<Genre> genres)
        {
            foreach (var genre in genres)
                book.Genres.Add(genre);

            await _dbContext.SaveChangesAsync();
        }

        public async Task AddGenreToBook(Book book, Genre genre)
        {
            book.Genres.Add(genre);

            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveGenreFromBook(Book book, Genre genre)
        {
            book.Genres.Remove(genre);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddAuthorToBook(Book book, Author author)
        {
            book.Authors.Add(author);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveAuthorFromBook(Book book, Author author)
        {
            book.Authors.Remove(author);
            await _dbContext.SaveChangesAsync();
        }
    }
}
