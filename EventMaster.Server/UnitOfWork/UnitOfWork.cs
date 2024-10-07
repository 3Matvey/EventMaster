using EventMaster.Server.Data;
using EventMaster.Server.Repositories.Implementation;
using EventMaster.Server.Repositories.Interfaces;
using EventMaster.Server.Services.Entities;

namespace EventMaster.Server.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IUserRepository? _userRepository;
        private IEventRepository? _eventRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);
        public IEventRepository EventRepository => _eventRepository ??= new EventRepository(_context);

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
