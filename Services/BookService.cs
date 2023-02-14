﻿using BlazingBooks.Data;
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

        public async Task<List<Book>> GetBooksFromDBAsync()
        {
            return await Task.Run(() => _dbContext.Books.Include(b => b.Authors).Include(b => b.Genres).ToList());
        }

        public async Task<Book> GetBookById(int id)
        {
            return await _dbContext.Books.FindAsync(id);
        }

        public async Task<List<Book>> GetBooksByGenre(Genre genre)
        {
            return await Task.Run(() => GetBooksFromDBAsync().Result.Where(b => b.Genres.Where(g => g.Name == genre.Name).Select(g => g).Count() > 0).Select(b => b).ToList());
        }

        public async Task<List<Book>> GetBooksByAuthor(Author author)
        {
            return await Task.Run(() => GetBooksFromDBAsync().Result.Where(b => b.Authors.Where(a => a.Name == author.Name).Select(a => a).Count() > 0).Select(b => b).ToList());
        }

        public async Task<List<Book>> GetBooksOrderedAsync(List<Book> books, PropertyEnum option)
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

        public async Task<List<Book>> GetBooksByPropertyAsync(List<Book> books, PropertyEnum option, object desiredValue)
        {
            return await Task.Run(() => option switch
            {
                //PropertyEnum.Author => books.Where(book => book.Authors.Where(a => a.Name.Contains((string)desiredValue)).Select(book => book).ToList(),
                PropertyEnum.Title => books.Where(book => book.Title.Contains((string)desiredValue)).Select(book => book).ToList(),
                PropertyEnum.Published => books.Where(book => book.Published == (DateTime)desiredValue).Select(book => book).ToList(),
                PropertyEnum.Price => books.Where(book => book.Price == (int)desiredValue).Select(book => book).ToList(),
                _ => throw new NotImplementedException()
            } ?? new List<Book>());
        }

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

        public async Task<Book> AddBookAsync(Book book)
        {
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();

            return book;
        }

        public async Task<Book> DeleteBookAsync(Book book)
        {
            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync();

            return book;
        }
    }
}
