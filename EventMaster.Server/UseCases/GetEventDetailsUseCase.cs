using EventMaster.Server.Entities;
using EventMaster.Server.UnitOfWork;

namespace EventMaster.Server.UseCases
{
    public class GetEventDetailsUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEventDetailsUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Event> Execute(int eventId)
        {
            var eventItem = await _unitOfWork.EventRepository.GetEventByIdAsync(eventId);
            if (eventItem is null)
            {
                throw new KeyNotFoundException("Event not found");
            }

            return eventItem;
        }
    }
}
