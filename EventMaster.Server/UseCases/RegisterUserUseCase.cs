using EventMaster.Server.Dto;
using EventMaster.Server.Entities;
using EventMaster.Server.UnitOfWork;

namespace EventMaster.Server.UseCases
{
    public class RegisterUserUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User> Execute(UserRegistrationDto userRegistration)
        {
            if (await _unitOfWork.UserRepository.UserExistsAsync(userRegistration.Email))
            {
                throw new InvalidOperationException("Email already in use");
            }

            var newUser = new User
            {
                FirstName = userRegistration.FirstName,
                LastName = userRegistration.LastName,
                BirthDate = userRegistration.BirthDate,
                Email = userRegistration.Email,
                Password = userRegistration.Password, // Пароль нужно хэшировать до этого вызова
                Role = "User",
                DateOf = DateTime.Now
            };

            await _unitOfWork.UserRepository.AddUserAsync(newUser);
            await _unitOfWork.SaveAsync();
            return newUser;
        }
    }
}
