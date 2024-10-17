using EventMaster.Server.Entities;
using EventMaster.Server.UnitOfWork;

namespace EventMaster.Server.UseCases
{
    public class GetRegisteredEventsUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRegisteredEventsUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Event>> Execute(string userEmail)
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(userEmail);
            return user?.RegisteredEvents ?? new List<Event>();
        }
    }
}
