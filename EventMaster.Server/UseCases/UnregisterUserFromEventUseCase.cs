using EventMaster.Server.UnitOfWork;

namespace EventMaster.Server.UseCases
{

    public class UnregisterUserFromEventUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnregisterUserFromEventUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Execute(int eventId, string userEmail)
        {
            var eventItem = await _unitOfWork.EventRepository.GetEventByIdAsync(eventId);
            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(userEmail);

            if (eventItem == null || user == null)
            {
                throw new KeyNotFoundException("Event or user not found");
            }

            if (!eventItem.Users.Any(u => u.Id == user.Id))
            {
                throw new InvalidOperationException("User is not registered for this event");
            }

            eventItem.Users.Remove(user);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}
