namespace BlazingBooks.Data;

public class OrderState
{
    public bool ShowingConfigureDialog { get; private set; }
    public Book ConfiguringBook { get; private set; }
    public Order Order { get; private set; } = new Order();

    public void ShowConfigureBookDialog(Book book)
    {
        ConfiguringBook = new Book()
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Published = book.Published,
            TotalCount = book.TotalCount,
            ToOrderCount = book.ToOrderCount,
            Price = book.Price
        };

        ShowingConfigureDialog = true;
    }

    public void CancelConfigureBookDialog()
    {
        ConfiguringBook = null;

        ShowingConfigureDialog = false;
    }

    public void ConfirmConfigureBookDialog()
    {
        Order.Books.Add(ConfiguringBook);
        ConfiguringBook = null;

        ShowingConfigureDialog = false;
    }

    public void RemoveConfiguredBook(Book book)
    {
        Order.Books.Remove(book);
    }
    
    public void ResetOrder()
    {
        Order.Books.Clear();
    }
}
