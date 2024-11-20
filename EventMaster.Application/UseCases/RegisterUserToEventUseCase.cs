namespace EventMaster.Application.UseCases
{
    public class RegisterUserToEventUseCase(IUnitOfWork unitOfWork)
    {
        public async Task<Result> Execute(int eventId, RegisterToEventDto dto)
        {
            var eventItem = await unitOfWork.EventRepository.GetEventByIdAsync(eventId);
            if (eventItem == null)
            {
                return Error.NotFound("EventNotFound", "Event not found.");
            }

            var user = await unitOfWork.UserRepository.GetUserByEmailAsync(dto.Email);
            if (user == null)
            {
                return Error.NotFound("UserNotFound", "User not found.");
            }

            if (eventItem.Users.Count >= eventItem.MaxMemberCount)
            {
                return Error.Validation("EventFull", "Event is full.");
            }

            if (eventItem.Users.Any(u => u.Id == user.Id))
            {
                return Error.Validation("AlreadyRegistered", "User is already registered for this event.");
            }

            eventItem.Users.Add(user);

            try
            {
                await unitOfWork.EventRepository.UpdateEventAsync(eventItem);
                await unitOfWork.SaveAsync();
                return Result.Success();
            }
            catch (Exception)
            {
                return Error.Failure("RegistrationFailed", "An error occurred while registering the user to the event.");
            }
        }
    }
}
