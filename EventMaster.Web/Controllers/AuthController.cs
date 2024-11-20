using Microsoft.AspNetCore.Mvc;
using EventMaster.Application.UseCases;
using EventMaster.Application.ResultPattern;
using EventMaster.Application.DTOs;

namespace EventMaster.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        RegisterUserUseCase registerUserUseCase,
        LoginUserUseCase loginUserUseCase) : BaseController
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto userRegistration)
        {
            var result = await registerUserUseCase.Execute(userRegistration);

            return result.Match(
                onSuccess: userDto => Ok(new { Message = "User registered successfully", User = userDto }),
                onFailure: Problem
            );
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLogin)
        {
            var result = await loginUserUseCase.Execute(userLogin);

            return result.Match(
                onSuccess: Ok,
                onFailure: Problem
            );
        }

        [HttpPost("update_tokens")]  
        public async Task<IActionResult> Refresh([FromBody] TokenRequestDto tokenRequest)
        {
            var principalResult = loginUserUseCase.GetPrincipalFromExpiredToken(tokenRequest.AccessToken);

            if (!principalResult.IsSuccess)
            {
                return Problem(principalResult.Error!);
            }

            var principal = principalResult.Value;

            var userResult = await loginUserUseCase.GetUserFromPrincipalAsync(principal, tokenRequest.RefreshToken);

            if (!userResult.IsSuccess)
            {
                return Problem(userResult.Error!);
            }

            var userDto = userResult.Value;

            var tokensResult = loginUserUseCase.GenerateNewTokens(principal);

            if (!tokensResult.IsSuccess)
            {
                return Problem(tokensResult.Error!);
            }

            var (AccessToken, RefreshToken) = tokensResult.Value;

            var updateResult = await loginUserUseCase.UpdateUserRefreshTokenAsync(userDto.Email, RefreshToken);

            if (!updateResult.IsSuccess)
            {
                return Problem(updateResult.Error!);
            }

            return Ok(new { token = AccessToken, refreshToken = RefreshToken });
        }
    }
}
