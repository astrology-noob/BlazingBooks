namespace BlazingBooks.Data;


// todo: привязать заказ к пользователю
public class OrderState
{
    public Order Order { get; private set; } = new Order();

    public void AddBookToCart(Book book)
    {
        if (Order.BooksList.ContainsKey(book))
        {
            Order.BooksList[book] += 1;
        }
        else
        {
            Order.BooksList.Add(book, 1);
        }
        OnBookAddedToCart();
    }

    public void RemoveBookFromCart(Book book)
    {
        Order.BooksList.Remove(book);
        OnBookRemovedFromCart();
    }
    
    public void ResetOrder()
    {
        Order.BooksList.Clear();
    }

    public event Action OnBookAddedToCart;
    public event Action OnBookRemovedFromCart;
}
