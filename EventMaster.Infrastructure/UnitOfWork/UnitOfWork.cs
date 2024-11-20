using EventMaster.Infrastructure.Data;
using EventMaster.Infrastructure.Repositories;
using EventMaster.Application.Interfaces.UnitOfWork;
using EventMaster.Application.Interfaces.Repositories;

namespace EventMaster.Infrastructure.UnitOfWork
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        private IUserRepository? _userRepository;
        private IEventRepository? _eventRepository;

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(context);
        public IEventRepository EventRepository => _eventRepository ??= new EventRepository(context);

        public async Task<int> SaveAsync()
        {
            return await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
