using EventMaster.Application.Interfaces.JwtService;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace EventMaster.Application.UseCases
{
    //public class LoginUserUseCase(
    //    IUnitOfWork unitOfWork,
    //    IOptions<JwtSettings> jwtSettings,
    //    IMapper mapper)
    //{
    //    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    //    public async Task<Result<LoginResultDto>> Execute(
    //        UserLoginDto userLogin)
    //    {
    //        var user = await unitOfWork.UserRepository
    //            .GetUserByEmailAsync(userLogin.Email);

    //        if (user == null || user.Password != userLogin.Password)
    //        {
    //            return Error.AccessUnauthorized(
    //                "InvalidCredentials",
    //                "Invalid email or password.");
    //        }

    //        // Создание JWT access токена
    //        var claims = new List<Claim>
    //        {
    //            new Claim(ClaimTypes.Name, user.Email),
    //            new Claim(ClaimTypes.Role, user.Role)
    //        };
    //        var token = GenerateAccessToken(claims);

    //        // Генерация refresh токена
    //        var refreshToken = GenerateRefreshToken();
    //        user.RefreshToken = refreshToken;
    //        user.RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(1);

    //        await unitOfWork.UserRepository.UpdateUserAsync(user);
    //        await unitOfWork.SaveAsync();

    //        var loginResult = new LoginResultDto
    //        {
    //            Token = token,
    //            RefreshToken = refreshToken,
    //            FirstName = user.FirstName,
    //            LastName = user.LastName,
    //            Email = user.Email,
    //            Role = user.Role
    //        };

    //        return Result<LoginResultDto>.Success(loginResult);
    //    }

    //    public Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(
    //        string token)
    //    {
    //        var tokenValidationParameters = new TokenValidationParameters
    //        {
    //            ValidateAudience = false,
    //            ValidateIssuer = false,
    //            ValidateIssuerSigningKey = true,
    //            IssuerSigningKey = new SymmetricSecurityKey(
    //                Encoding.UTF8.GetBytes(_jwtSettings.IssuerSigningKey)),
    //            ValidateLifetime = false
    //        };

    //        try
    //        {
    //            var tokenHandler = new JwtSecurityTokenHandler();
    //            var principal = tokenHandler.ValidateToken(
    //                token, tokenValidationParameters, out SecurityToken
    //                securityToken);

    //            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
    //                !jwtSecurityToken.Header.Alg.Equals(
    //                    SecurityAlgorithms.HmacSha256,
    //                    StringComparison.InvariantCultureIgnoreCase))
    //            {
    //                return Error.Validation(
    //                    "InvalidToken", "Invalid token.");
    //            }

    //            return Result<ClaimsPrincipal>.Success(principal);
    //        }
    //        catch (Exception)
    //        {
    //            return Error.Failure(
    //                "TokenValidationError",
    //                "An error occurred while validating the token.");
    //        }
    //    }

    //    public async Task<Result<UserDto>> GetUserFromPrincipalAsync(
    //        ClaimsPrincipal principal, string refreshToken)
    //    {
    //        var email = principal.Identity?.Name;
    //        if (string.IsNullOrEmpty(email))
    //        {
    //            return Error.Validation(
    //                "InvalidPrincipal", "Principal does not contain email.");
    //        }

    //        var user = await unitOfWork.UserRepository
    //            .GetUserByEmailAsync(email);

    //        if (user == null)
    //        {
    //            return Error.NotFound("UserNotFound", "User not found.");
    //        }

    //        if (user.RefreshToken != refreshToken ||
    //            user.RefreshTokenExpiryDate <= DateTime.UtcNow)
    //        {
    //            return Error.Validation(
    //                "InvalidRefreshToken",
    //                "Invalid or expired refresh token.");
    //        }

    //        var userDto = mapper.Map<UserDto>(user);
    //        return Result<UserDto>.Success(userDto);
    //    }

    //    public Result<(string AccessToken, string RefreshToken)>
    //        GenerateNewTokens(ClaimsPrincipal principal)
    //    {
    //        try
    //        {
    //            var claims = principal.Claims;
    //            var accessToken = GenerateAccessToken(claims);
    //            var refreshToken = GenerateRefreshToken();

    //            return Result<(string, string)>.Success(
    //                (accessToken, refreshToken));
    //        }
    //        catch (Exception)
    //        {
    //            return Error.Failure(
    //                "TokenGenerationError",
    //                "An error occurred while generating new tokens.");
    //        }
    //    }

    //    public async Task<Result> UpdateUserRefreshTokenAsync(
    //        string email, string refreshToken)
    //    {
    //        var user = await unitOfWork.UserRepository
    //            .GetUserByEmailAsync(email);

    //        if (user == null)
    //        {
    //            return Error.NotFound("UserNotFound", "User not found.");
    //        }

    //        user.RefreshToken = refreshToken;
    //        user.RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(7);

    //        await unitOfWork.UserRepository.UpdateUserAsync(user);
    //        await unitOfWork.SaveAsync();

    //        return Result.Success();
    //    }

    //    private string GenerateAccessToken(IEnumerable<Claim> claims)
    //    {
    //        var key = new SymmetricSecurityKey(
    //            Encoding.UTF8.GetBytes(_jwtSettings.IssuerSigningKey));
    //        var creds = new SigningCredentials(
    //            key, SecurityAlgorithms.HmacSha256);

    //        var token = new JwtSecurityToken(
    //            issuer: _jwtSettings.ValidIssuer,
    //            audience: _jwtSettings.ValidAudience,
    //            claims: claims,
    //            expires: DateTime.UtcNow.AddMinutes(30),
    //            signingCredentials: creds);

    //        return new JwtSecurityTokenHandler().WriteToken(token);
    //    }

    //    private static string GenerateRefreshToken()
    //    {
    //        var randomBytes = new byte[32];
    //        using var rng = RandomNumberGenerator.Create();
    //        rng.GetBytes(randomBytes);
    //        return Convert.ToBase64String(randomBytes);
    //    }
    //}
    public class LoginUserUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMapper _mapper;

        public LoginUserUseCase(
            IUnitOfWork unitOfWork,
            IJwtTokenService jwtTokenService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
            _mapper = mapper;
        }

        public async Task<Result<LoginResultDto>> Execute(UserLoginDto userLogin)
        {
            var user = await _unitOfWork.UserRepository
                .GetUserByEmailAsync(userLogin.Email);

            if (user == null || !VerifyPassword(userLogin.Password, user.Password))
            {
                return Error.AccessUnauthorized(
                    "InvalidCredentials",
                    "Invalid email or password.");
            }

            // Создание JWT access токена
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var token = _jwtTokenService.GenerateAccessToken(claims);

            // Генерация refresh токена
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(1);

            await _unitOfWork.UserRepository.UpdateUserAsync(user);
            await _unitOfWork.SaveAsync();

            var loginResult = new LoginResultDto
            {
                Token = token,
                RefreshToken = refreshToken,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role
            };

            return Result<LoginResultDto>.Success(loginResult);
        }

        private static bool VerifyPassword(string password, string passwordHash)
        {
            // могло бы быть хеширование
            return password == passwordHash;
        }

        public Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var principal = _jwtTokenService.GetPrincipalFromExpiredToken(token);
                return Result<ClaimsPrincipal>.Success(principal);
            }
            catch (SecurityTokenException ex)
            {
                return Error.Validation("InvalidToken", ex.Message);
            }
            catch (Exception)
            {
                return Error.Failure(
                    "TokenValidationError",
                    "An error occurred while validating the token.");
            }
        }

        public async Task<Result<UserDto>> GetUserFromPrincipalAsync(
            ClaimsPrincipal principal, string refreshToken)
        {
            var email = principal.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return Error.Validation(
                    "InvalidPrincipal", "Principal does not contain email.");
            }

            var user = await _unitOfWork.UserRepository
                .GetUserByEmailAsync(email);

            if (user == null)
            {
                return Error.NotFound("UserNotFound", "User not found.");
            }

            if (user.RefreshToken != refreshToken ||
                user.RefreshTokenExpiryDate <= DateTime.UtcNow)
            {
                return Error.Validation(
                    "InvalidRefreshToken",
                    "Invalid or expired refresh token.");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return Result<UserDto>.Success(userDto);
        }

        public Result<(string AccessToken, string RefreshToken)> GenerateNewTokens(ClaimsPrincipal principal)
        {
            try
            {
                var claims = principal.Claims;
                var accessToken = _jwtTokenService.GenerateAccessToken(claims);
                var refreshToken = _jwtTokenService.GenerateRefreshToken();

                return Result<(string, string)>.Success(
                    (accessToken, refreshToken));
            }
            catch (Exception)
            {
                return Error.Failure(
                    "TokenGenerationError",
                    "An error occurred while generating new tokens.");
            }
        }

        public async Task<Result> UpdateUserRefreshTokenAsync(
            string email, string refreshToken)
        {
            var user = await _unitOfWork.UserRepository
                .GetUserByEmailAsync(email);

            if (user == null)
            {
                return Error.NotFound("UserNotFound", "User not found.");
            }

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(7);

            await _unitOfWork.UserRepository.UpdateUserAsync(user);
            await _unitOfWork.SaveAsync();

            return Result.Success();
        }
    }
}
