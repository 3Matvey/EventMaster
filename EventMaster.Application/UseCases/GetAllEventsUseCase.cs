namespace EventMaster.Application.UseCases
{
    public class GetAllEventsUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        public async Task<Result<List<EventDto>>> Execute()
        {
            var events = await unitOfWork.EventRepository.GetAllEventsAsync();

            if (events == null || events.Count == 0)
            {
                return Error.NotFound("NoEventsFound", "No events found.");
            }

            var eventDtos = mapper.Map<List<EventDto>>(events);

            return Result<List<EventDto>>.Success(eventDtos);
        }
    }
}
