using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazingBooks.Data
{
    public class BookService
    {
        private AppDBContext _dbContext;

        public BookService(AppDBContext db)
        {
            _dbContext = db;
        }

        public async Task<List<Book>> GetBooksFromDBAsync()
        {
            return await Task.Run(() => _dbContext.Books.ToList());
        }

        public async Task<List<Book>> GetBooksOrderedAsync(List<Book> books, PropertyEnum option)
        {
            return await Task.Run(() => option switch
            {
                PropertyEnum.Author    => books.OrderBy(book => book.Author).ToList(),
                PropertyEnum.Title     => books.OrderBy(book => book.Title).ToList(),
                PropertyEnum.Published => books.OrderBy(book => book.Published).ToList(),
                PropertyEnum.Price => books.OrderBy(book => book.Price).ToList(),
                _ => throw new NotImplementedException()
            });
        }

        public async Task<List<Book>> GetBooksByPropertyAsync(List<Book> books, PropertyEnum option, object desiredValue)
        {
            return await Task.Run(() => option switch
            {
                PropertyEnum.Author => books.Where(book => book.Author.Contains((string)desiredValue)).Select(book => book).ToList(),
                PropertyEnum.Title => books.Where(book => book.Title.Contains((string)desiredValue)).Select(book => book).ToList(),
                PropertyEnum.Published => books.Where(book => book.Published == (DateTime)desiredValue).Select(book => book).ToList(),
                PropertyEnum.Price => books.Where(book => book.Price == (int)desiredValue).Select(book => book).ToList(),
                _ => throw new NotImplementedException()
            } ?? new List<Book>());
        }

        public async Task BuyBooks(KeyValuePair <Book, int> bookCount)
        {
            Book bookToAlter = _dbContext.Books.Find(bookCount.Key.Id);
            if (bookToAlter.TotalCount > bookCount.Value) 
            {
                throw new Exception();
            }
            bookToAlter.TotalCount -= bookCount.Value;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();

            return book;
        }

        // buy
        public async Task<Book> DeleteBookAsync(Book book)
        {
            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync();

            return book;
        }
    }
}
