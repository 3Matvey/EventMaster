using EventMaster.Domain.Entities;
using EventMaster.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using EventMaster.Application.Interfaces.Repositories;

namespace EventMaster.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        private readonly AppDbContext _context = context;

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.Include(u => u.RegisteredEvents).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.Include(u => u.RegisteredEvents).FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.Include(u => u.RegisteredEvents).ToListAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            await Task.Run(() =>
            {
                _context.Users.Update(user);
            });
        }

        public async Task DeleteUserAsync(User user)
        {
            await Task.Run(() =>
            {
                _context.Users.Remove(user);
            });
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}
