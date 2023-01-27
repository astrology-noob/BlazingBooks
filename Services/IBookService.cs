using BlazingBooks.Data;

namespace BlazingBooks.Services
{
    public interface IBookService
    {
        Task<List<Book>> GetBooksFromDBAsync();

        Task<List<Book>> GetBooksOrderedAsync(List<Book> books, PropertyEnum option);

        Task<List<Book>> GetBooksByPropertyAsync(List<Book> books, PropertyEnum option, object desiredValue);

        Task<bool> BuyBooks(KeyValuePair<Book, int> bookCount);

        Task<Book> AddBookAsync(Book book);

        Task<Book> DeleteBookAsync(Book book);
    }
}
