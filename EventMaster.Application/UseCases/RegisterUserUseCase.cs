namespace EventMaster.Application.UseCases
{
    public class RegisterUserUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        public async Task<Result<UserDto>> Execute(UserRegistrationDto userRegistration)
        {
            if (await unitOfWork.UserRepository.UserExistsAsync(userRegistration.Email))
            {
                return Error.Validation("EmailInUse", "Email already in use.");
            }

            //var hashedPassword = HashPassword(userRegistration.Password);

            var newUser = mapper.Map<User>(userRegistration);
            newUser.Password = userRegistration.Password;  
            newUser.Role = "User";
            newUser.DateOf = DateTime.UtcNow;

            try
            {
                await unitOfWork.UserRepository.AddUserAsync(newUser);
                await unitOfWork.SaveAsync();

                var userDto = mapper.Map<UserDto>(newUser);
                return Result<UserDto>.Success(userDto);
            }
            catch (Exception)
            {
                return Error.Failure("RegistrationFailed", "An error occurred while registering the user.");
            }
        }

        //private static string HashPassword(string password)
        //{
        //    return BCrypt.Net.BCrypt.HashPassword(password);
        //}
    }
}


