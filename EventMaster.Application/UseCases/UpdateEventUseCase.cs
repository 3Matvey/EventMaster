namespace EventMaster.Application.UseCases
{
    public class UpdateEventUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        public async Task<Result<EventDto>> Execute(int eventId, EventDto eventDto)
        {
            eventDto.Id = eventId;

            var existingEvent = await unitOfWork.EventRepository.GetEventByIdAsync(eventId);
            if (existingEvent == null)
            {
                return Error.NotFound("EventNotFound", "Event not found.");
            }

            mapper.Map(eventDto, existingEvent);

            try
            {
                await unitOfWork.EventRepository.UpdateEventAsync(existingEvent);
                //await unitOfWork.EventRepository.UpdateEventImagePathAsync(existingEvent.ImagePath);
                await unitOfWork.SaveAsync();

                var updatedEventDto = mapper.Map<EventDto>(existingEvent);
                return Result<EventDto>.Success(updatedEventDto);
            }
            catch (Exception)
            {
                return Error.Failure("UpdateFailed", "An error occurred while updating the event.");
            }
        }
    }
}
