namespace BlazingBooks.Data;

public class Order
{
    public int OrderId { get; set; }

    public string UserId { get; set; }

    public DateTime CreatedTime { get; set; }

    public Dictionary<Book, int> Books { get; set; } = new Dictionary<Book, int>();

    public decimal GetTotalPrice() => Books.Sum(booksCount => booksCount.Key.Price * booksCount.Value);

    public string GetFormattedTotalPrice() => GetTotalPrice().ToString("0.00");
}
