using System.ComponentModel.DataAnnotations;

namespace BlazingBooks.Data
{
    public sealed record Genre()
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Book> Books { get; set; }

        public Genre(string name, ICollection<Book> books) : this()
        {
            Name = name;
            Books = books;
        }
    }
}
