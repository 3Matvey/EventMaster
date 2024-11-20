namespace EventMaster.Application.UseCases
{
    public class GetEventDetailsUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        public async Task<Result<EventDto>> Execute(int eventId)
        {
            var eventItem = await unitOfWork.EventRepository.GetEventByIdAsync(eventId);
            if (eventItem == null)
            {
                return Error.NotFound("EventNotFound", "Event not found.");
            }

            var eventDto = mapper.Map<EventDto>(eventItem);

            return Result<EventDto>.Success(eventDto);
        }
    }
}
