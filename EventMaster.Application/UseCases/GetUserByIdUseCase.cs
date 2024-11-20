namespace EventMaster.Application.UseCases
{
    public class GetUserByIdUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        public async Task<Result<UserDto>> Execute(int userId)
        {
            var user = await unitOfWork.UserRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Error.NotFound("UserNotFound", "User not found.");
            }
            var userDto = mapper.Map<UserDto>(user);

            return Result<UserDto>.Success(userDto);
        }
    }
}
