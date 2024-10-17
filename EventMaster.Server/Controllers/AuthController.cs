using Microsoft.AspNetCore.Mvc;
using EventMaster.Server.Dto;
using EventMaster.Server.UseCases;

namespace EventMaster.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RegisterUserUseCase _registerUserUseCase;
        private readonly LoginUserUseCase _loginUserUseCase;

        public AuthController(RegisterUserUseCase registerUserUseCase, LoginUserUseCase loginUserUseCase, JwtSettings jwtSettings)
        {
            _registerUserUseCase = registerUserUseCase;
            _loginUserUseCase = loginUserUseCase;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto userRegistration)
        {
            var newUser = await _registerUserUseCase.Execute(userRegistration);
            return Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLogin)
        {
            var result = await _loginUserUseCase.Execute(userLogin);
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenRequestDto tokenRequest)
        {
            var principal = _loginUserUseCase.GetPrincipalFromExpiredToken(tokenRequest.AccessToken);
            var user = await _loginUserUseCase.GetUserFromPrincipal(principal, tokenRequest.RefreshToken);

            // Генерация нового access токена и refresh токена
            var result = _loginUserUseCase.GenerateNewTokens(principal, user!);
            await _loginUserUseCase.UpdateUserRefreshTokenAsync(user!, result.refreshToken);

            return Ok(new
            {
                token = result.accessToken,
                refreshToken = result.refreshToken
            });
        }
    }
}
