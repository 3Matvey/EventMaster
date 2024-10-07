using EventMaster.Server.Services.Entities;
using EventMaster.Server.UnitOfWork;

public class UserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddUserAsync(User newUser)
    {
        await _unitOfWork.UserRepository.AddUserAsync(newUser);
        await _unitOfWork.SaveAsync();
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _unitOfWork.UserRepository.GetAllUsersAsync();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _unitOfWork.UserRepository.GetUserByEmailAsync(email);
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _unitOfWork.UserRepository.GetUserByIdAsync(id);
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        var success = await _unitOfWork.UserRepository.UpdateUserAsync(user);
        if (success)
        {
            await _unitOfWork.SaveAsync();
        }
        return success;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var success = await _unitOfWork.UserRepository.DeleteUserAsync(id);
        if (success)
        {
            await _unitOfWork.SaveAsync();
        }
        return success;
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        return await _unitOfWork.UserRepository.UserExistsAsync(email);
    }
}
