using BlazingBooks.Data;
using BlazingBooks.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazingBooks.Controllers
{
    class UserSameName : EqualityComparer<User>
    {
        public override bool Equals(User x, User y)
        {
            if (x.Username == y.Username) return true;
            else return false;
        }

        public override int GetHashCode([DisallowNull] User user)
        {
            return user.Username.GetHashCode();
        }
    }

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
        public ActionResult<RegisterResult> Register([FromBody] UserData userData)
        {
            string passwordHash
                = BCrypt.Net.BCrypt.HashPassword(userData.Password);

            user.Username = userData.Username;
            user.PasswordHash = passwordHash;

            UserSameName userSameName = new();
            var userExists = _context.Users.ToList().Contains(user, userSameName);

			if (userExists)
            {
                return BadRequest(new RegisterResult { Successful = false, Errors = new string[1] { "User already exists" } });
            }

            _context.Users.Add(user);
            _context.SaveChangesAsync();

            return Ok(new RegisterResult { Successful = true });
        }

        [HttpPost("login")]
        public ActionResult<LoginResult> Login([FromBody] UserData userData)
        {
            user = _context.Users.FirstOrDefault(u => u.Username == userData.Username);
            if (user is null)
            {
                return BadRequest(new LoginResult { Successful = false, Error = "User not found" });
            }

            if (!BCrypt.Net.BCrypt.Verify(userData.Password, user.PasswordHash))
            {
                return BadRequest(new LoginResult { Successful = false, Error = "Wrong password" });
            }

            user = new()
            {
                Username = userData.Username,
                PasswordHash = userData.Password
            };

            string token = CreateToken(user);

            return Ok(new LoginResult { Successful = true, Token = token });
        }

        private string CreateToken(User user)
        {

            //var roles = user.GetRolesAsync(user);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            //foreach (var role in roles)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, role));
            //}

            var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtExpiryInDays"]));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtAudience"],
                claims,
                expires: expiry,
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}