using BlazingBooks.Data;
using BlazingBooks.Services;
using BlazingBooks.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

using System.Text.Json;

namespace BlazingBooks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();
        private readonly IConfiguration _configuration;

        private readonly AppDBContext _context;
        public AuthController(AppDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<string> Register(StringContent request)
        {
			Task<string> readingTask = request.ReadAsStringAsync();
			string data = await readingTask;
			readingTask.Wait();

            //UserData userData = JsonSerializer.Deserialize<UserData>(data);

			//string passwordHash
   //             = BCrypt.Net.BCrypt.HashPassword(userData.Password);

   //         user.Username = userData.Username;
   //         user.PasswordHash = passwordHash;

            //_context.Users.Add(user);
            //_context.SaveChangesAsync();
            //user = new User();

            return "a";
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(StringContent request)
        {
            Task<string> readingTask = request.ReadAsStringAsync();
            string data = await readingTask;
            readingTask.Wait();

			//UserData userData = JsonSerializer.Deserialize<UserData>(data);

			//user = (User) _context.Users.Where(u => u.Username == userData.Username).Select(u => u);
			//         if (user is null)
			//         {
			//             return BadRequest("User not found.");
			//         }

			//         if (!BCrypt.Net.BCrypt.Verify(userData.Password, user.PasswordHash))
			//         {
			//             return BadRequest("Wrong password.");
			//         }

			//user.Username = userData.Username;
   //         user.PasswordHash = userData.Password;

			//string token = CreateToken(user);

            //user = new User();

            return Ok(data);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}