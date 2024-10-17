using EventMaster.Server.Entities;
using EventMaster.Server.UnitOfWork;

namespace EventMaster.Server.UseCases
{
    public class GetAllEventsUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllEventsUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Event>> Execute()
        {
            return await _unitOfWork.EventRepository.GetAllEventsAsync();
        }
    }
}
