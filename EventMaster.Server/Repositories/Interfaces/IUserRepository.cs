using EventMaster.Server.Services.Entities;

namespace EventMaster.Server.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<List<User>> GetAllUsersAsync();
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> UserExistsAsync(string email);
    }
}
