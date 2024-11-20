namespace EventMaster.Application.UseCases
{
    public class GetRegisteredEventsUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        public async Task<Result<List<EventDto>>> Execute(string userEmail)
        {
            var user = await unitOfWork.UserRepository.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return Error.NotFound("UserNotFound", "User not found.");
            }

            var events = user.RegisteredEvents;

            if (events == null || events.Count == 0)
            {
                return Error.NotFound("NoRegisteredEvents", "User is not registered for any events.");
            }

            var eventDtos = mapper.Map<List<EventDto>>(events);

            return Result<List<EventDto>>.Success(eventDtos);
        }
    }
}
