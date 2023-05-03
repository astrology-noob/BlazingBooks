using System.ComponentModel.DataAnnotations;

namespace BlazingBooks.Data
{
    public sealed record Book()
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public ICollection<Author> Authors { get; set; }
        public DateTime Published { get; set; } = DateTime.Now;
        public string CoverImageUrl { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int TotalCount { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public string GetFormattedBasePrice() => Price.ToString("0.00");

        public Book(string title, ICollection<Author> authors, DateTime published, string coverImageUrl, 
                    string description, int price, int totalCount, ICollection<Genre> genres) : this()
        {
            Title = title;
            Authors = authors;
            Published = published;
            CoverImageUrl = coverImageUrl;
            Description = description;
            Price = price;
            TotalCount = totalCount;
            Genres = genres;
        }
    }
}
