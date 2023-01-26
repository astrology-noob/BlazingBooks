namespace BlazingBooks.Data;

public class OrderState
{
    public bool ShowingConfigureDialog { get; private set; }
    public Book ConfiguringBook { get; set; }
    public int ConfiguringBookCount { get; set; }
    public Order Order { get; private set; } = new Order();

    public void ShowConfigureBookDialog(Book book, int count)
    {
        ConfiguringBook = book;
        ConfiguringBookCount = count;
        
        ShowingConfigureDialog = true;
    }

    public void CancelConfigureBookDialog()
    {
        ConfiguringBook = null;
        ConfiguringBookCount = 0;

        ShowingConfigureDialog = false;
    }

    public void ConfirmConfigureBookDialog(int count)
    {
        ConfiguringBookCount = count;
        if (Order.Books.ContainsKey(ConfiguringBook))
        {
            Order.Books[ConfiguringBook] += ConfiguringBookCount;
        }
        else
        {
            Order.Books.Add(ConfiguringBook, ConfiguringBookCount);
        }

        ConfiguringBook = null;
        ConfiguringBookCount = 0;

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
