namespace EventMaster.Application.UseCases
{
    public class CreateEventUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        public async Task<Result<EventDto>> Execute(EventDto eventDto)
        {
            if (eventDto == null)
            {
                return Error.Validation("InvalidData", "Event data is null.");
            }

            if (string.IsNullOrEmpty(eventDto.Name) ||
                string.IsNullOrEmpty(eventDto.Description) ||
                string.IsNullOrEmpty(eventDto.Place) ||
                eventDto.Date == default ||
                eventDto.MaxMemberCount <= 0)
            {
                return Error.Validation("InvalidData", "Please ensure all fields are filled correctly.");
            }

            var newEvent = mapper.Map<Event>(eventDto);

            try
            {
                await unitOfWork.EventRepository.AddEventAsync(newEvent);
                await unitOfWork.SaveAsync();

                var createdEventDto = mapper.Map<EventDto>(newEvent);

                return Result<EventDto>.Success(createdEventDto);
            }
            catch (Exception)
            {
                return Error.Failure("CreateEventError", "An error occurred while creating the event.");
            }
        }
    }
}
