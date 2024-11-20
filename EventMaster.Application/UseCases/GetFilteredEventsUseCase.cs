namespace EventMaster.Application.UseCases
{
    public class GetFilteredEventsUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        public async Task<Result<List<EventDto>>> Execute(string? name, DateTime? date, string? type, string? place, int pageNumber, int pageSize)
        {
            var events = await unitOfWork.EventRepository.GetFilteredEventsAsync(name, date, type, place, pageNumber, pageSize);

            var eventDtos = mapper.Map<List<EventDto>>(events);

            return Result<List<EventDto>>.Success(eventDtos);
        }
    }
}
