namespace BlazingBooks.Shared
{
    
    public class UserData
    {
        public string Username { get; set; }
        public string Password { get; set; }

		public UserData()
        {
            Username = "a";
            Password ="a";
        }

		public UserData(string username, string password)
        {
            Username= username;
            Password= password;
        }

	}
}
