# 👯♀ Модель базы данных

В проекте существует 5 основных сущностей:

* Для каталога книжек:
  * Книга (Book)
  * Автор (Author)
  * Жанр (Genre)
* Для системы авторизации:
  * Пользователь (User)
  * Роль (Role)

Сущности связаны подобным образом:

Связи книга-жанр, книга-автор и пользователь-роль являются связами "многие-ко-многим". Эти связи реализованы с помощью промежуточных таблиц с первичными ключами соответствующих записей из связываемых сущностей.

<figure><img src="../.gitbook/assets/image (6).png" alt=""><figcaption></figcaption></figure>

База данных создавалась code-first. Далее представлены модели сущностей:

```csharp
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
```

```csharp
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
```

```csharp
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
```

```csharp
public class User
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public ICollection<Role> Roles { get; set; }
}
```

```csharp
public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<User> Users { get; set; }
}
```

Для работы с записями почти каждой сущности в базе данных созданы сервисы.

Например, таким образом выглядит сервис для взаимодействия с жанрами:

```csharp
public class GenreService
{
    private AppDBContext _dbContext;

    public GenreService(AppDBContext db)
    {
        _dbContext = db;
    }

    // этот метод используется для получения полного списка жанров,
    // который не отслеживается контекстом,
    // чтобы устранить конфликты при получении списка жанров более одного раза на сайте
    public async Task<List<Genre>> GetGenresReadOnly()
    {
        return await Task.Run(() => 
        _dbContext.Genres.AsNoTrackingWithIdentityResolution()
        .Include(g => g.Books).ToList());
    }

    public async Task<List<Genre>> GetGenresFromDBAsync()
    {
        return await Task.Run(() => 
        _dbContext.Genres
        .Include(g => g.Books).ToList());
    }

    public async Task<List<Genre>> GetGenreNames()
    {
        return await Task.Run(() => 
        _dbContext.Genres
        .AsNoTracking().Select(g => new Genre { Name = g.Name }).ToList());
    }

    // CRUD операции с записями
    public async Task<Genre> GetGenreByName(string genreName)
    {
        return await Task.Run(() => 
        _dbContext.Genres.FirstOrDefault(g => g.Name == genreName));
    }

    public async Task<Genre> GetGenreById(int id)
    {
        return await Task.Run(() => 
        _dbContext.Genres
        .AsNoTracking().FirstOrDefault(g => g.Id == id));
    }

    public async Task UpdateGenreAsync(Genre genre)
    {
        _dbContext.Update(genre);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddGenreAsync(Genre genre)
    {
        _dbContext.Add(genre);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveGenreAsync(Genre genre)
    {
        _dbContext.Remove(genre);
        await _dbContext.SaveChangesAsync();
    }
}
```
