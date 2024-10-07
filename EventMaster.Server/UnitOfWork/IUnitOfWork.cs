using EventMaster.Server.Repositories.Interfaces;
using EventMaster.Server.Services.Entities;

namespace EventMaster.Server.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IEventRepository EventRepository { get; }
        Task<int> SaveAsync();
    }
}
