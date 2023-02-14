using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace BlazingBooks.Data
{
    public sealed record Author()
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Book> Books { get; set; }

        public Author(string name, ICollection<Book> books) : this()
        {
            Name = name;
            Books = books;
        }
    }
}