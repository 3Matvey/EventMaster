using EventMaster.Server.Services;
using EventMaster.Server.Services.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventMaster.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserService _userService;

        public AuthController(IOptions<JwtSettings> jwtSettings, UserService userService)
        {
            _jwtSettings = jwtSettings.Value;
            _userService = userService;
        }

        [HttpPost("register")]
        public IActionResult Register(UserRegistrationDto userRegistration)
        {
            // Проверка, если пользователь с таким email уже существует
            if (_userService.UserExists(userRegistration.Email))
            {
                return BadRequest(new { Message = "Email already in use" });
            }

            var newUser = new User
            {
                FirstName = userRegistration.FirstName,
                LastName = userRegistration.LastName,
                BirthDate = userRegistration.BirthDate,
                Email = userRegistration.Email,
                Password = userRegistration.Password,  // Важно позже добавить хэширование пароля
                Role = "User",  // По умолчанию присваиваем роль "User"
                DateOf = DateTime.Now
            };

            // Добавляем пользователя через сервис
            _userService.AddUser(newUser);

            return Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("login")]
        public IActionResult Login(UserLoginDto userLogin)
        {
            var user = _userService.GetUserByEmail(userLogin.Email);

            if (user == null || user.Password != userLogin.Password)
            {
                return Unauthorized(new { Message = "Invalid email or password" });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.IssuerSigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.ValidIssuer,
                audience: _jwtSettings.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                firstName = user.FirstName,   
                lastName = user.LastName,
                email = user.Email, ///
                role = user.Role
            });
        } 
    }

    public class UserRegistrationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class JwtSettings
    {
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public string IssuerSigningKey { get; set; }
    }
}
