using System;
using System.ComponentModel.DataAnnotations;

namespace BlazingBooks.Data
{
    public sealed record Book()
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime Published { get; set; } = DateTime.Now;
        public int Price { get; set; }
        public int TotalCount { get; set; }
        public int ToOrderCount { get; set; } = 1;
        public string GetFormattedBasePrice() => Price.ToString("0.00");
        public int GetTotalPrice() => ToOrderCount*Price;
        public string GetFormattedTotalPrice() => GetTotalPrice().ToString("0.00");

        public Book(string title, string author, DateTime published, int price, int totalCount) : this()
        {
            Title = title;
            Author = author;
            Published = published;
            Price = price;
            TotalCount = totalCount;
        }
    }
}
