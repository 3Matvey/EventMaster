using EventMaster.Server.UnitOfWork;

namespace EventMaster.Server.UseCases
{
    public class DeleteEventUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteEventUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Execute(int eventId)
        {
            var eventItem = await _unitOfWork.EventRepository.GetEventByIdAsync(eventId);
            if (eventItem == null)
            {
                throw new KeyNotFoundException("Event not found");
            }

            //await _unitOfWork.EventRepository.DeleteEventAsync(eventId);
            await _unitOfWork.EventRepository.DeleteEventAsync(eventItem);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}
