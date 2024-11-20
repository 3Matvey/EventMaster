namespace EventMaster.Application.UseCases
{
    public class UnregisterUserFromEventUseCase(IUnitOfWork unitOfWork)
    {
        public async Task<Result> Execute(int eventId, string userEmail)
        {
            var eventItem = await unitOfWork.EventRepository.GetEventByIdAsync(eventId);
            if (eventItem == null)
            {
                return Error.NotFound("EventNotFound", "Event not found.");
            }

            var user = await unitOfWork.UserRepository.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return Error.NotFound("UserNotFound", "User not found.");
            }

            if (!eventItem.Users.Any(u => u.Id == user.Id))
            {
                return Error.Validation("UserNotRegistered", "User is not registered for this event.");
            }

            eventItem.Users.Remove(user);

            try
            {
                await unitOfWork.EventRepository.UpdateEventAsync(eventItem);
                await unitOfWork.SaveAsync();
                return Result.Success();
            }
            catch (Exception)
            {
                return Error.Failure("UnregistrationFailed", "An error occurred while unregistering the user from the event.");
            }
        }
    }
}
