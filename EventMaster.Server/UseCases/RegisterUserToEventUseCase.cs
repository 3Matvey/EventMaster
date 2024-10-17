using EventMaster.Server.UnitOfWork;

namespace EventMaster.Server.UseCases
{
    public class RegisterUserToEventUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserToEventUseCase(IUnitOfWork unitOfWork)
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

            if (eventItem.Users.Count >= eventItem.MaxMemberCount)
            {
                throw new InvalidOperationException("Event is full");
            }

            if (eventItem.Users.Any(u => u.Id == user.Id))
            {
                throw new InvalidOperationException("User is already registered");
            }

            eventItem.Users.Add(user);
            await _unitOfWork.EventRepository.UpdateEventAsync(eventItem);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}
