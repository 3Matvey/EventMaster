using EventMaster.Server.Services;
using EventMaster.Server.Services.Entities;
using EventMaster.Server.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
        public async Task<IActionResult> Register(UserRegistrationDto userRegistration)
        {
            // Проверка, если пользователь с таким email уже существует
            if (await _userService.UserExistsAsync(userRegistration.Email))
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

            await _userService.AddUserAsync(newUser);

            return Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLogin)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(userLogin.Email);
                if (user == null || user.Password != userLogin.Password)
                {
                    return Unauthorized(new { Message = "Invalid email or password" });
                }

                // Создание JWT access токена
                // Создание JWT access токена
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };
                var token = GenerateAccessToken(claims);

                // Генерация refresh токена
                var refreshToken = GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryDate = DateTime.Now.AddDays(1); // Действителен день
                await _userService.UpdateUserAsync(user);

                return Ok(new
                {
                    token = token,
                    refreshToken = refreshToken,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    role = user.Role
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging in: {ex.Message}");
                return StatusCode(500, new { Message = "An internal server error occurred. Please try again later." });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenRequestDto tokenRequest)
        {
            if (tokenRequest is null)
            {
                return BadRequest(new { Message = "Invalid request" });
            }

            var principal = GetPrincipalFromExpiredToken(tokenRequest.AccessToken);
            if (principal == null)
            {
                return BadRequest(new { Message = "Invalid access token or refresh token" });
            }

            var user = await _userService.GetUserByEmailAsync(principal.Identity.Name);
            if (user == null || user.RefreshToken != tokenRequest.RefreshToken || user.RefreshTokenExpiryDate <= DateTime.Now)
            {
                return BadRequest(new { Message = "Invalid access token or refresh token" });
            }

            // Генерация нового access токена
            var newAccessToken = GenerateAccessToken(principal.Claims);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryDate = DateTime.Now.AddDays(7);

            await _userService.UpdateUserAsync(user);

            return Ok(new
            {
                token = newAccessToken,
                refreshToken = newRefreshToken
            });
        }
        private string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.IssuerSigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.ValidIssuer,
                audience: _jwtSettings.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, // you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.IssuerSigningKey)),
                ValidateLifetime = false // here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        // DTO Classes
        public class TokenRequestDto
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
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

        // JWT Settings Configuration
        public class JwtSettings
        {
            public string ValidIssuer { get; set; }
            public string ValidAudience { get; set; }
            public string IssuerSigningKey { get; set; }
        }
    }
}
