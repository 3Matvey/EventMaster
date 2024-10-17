using EventMaster.Server.Repositories.Interfaces;

namespace EventMaster.Server.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IEventRepository EventRepository { get; }
        Task<int> SaveAsync();
    }
}
