namespace BlazingBooks.Data
{
    public class UserData
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public UserData(string username, string password)
        {
            Username = username;
            Password = password;
        }

    }
}
