using EventMaster.Application.Interfaces.Repositories;

namespace EventMaster.Application.Interfaces.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IEventRepository EventRepository { get; }
        Task<int> SaveAsync();
    }
}
