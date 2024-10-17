using AutoMapper;
using EventMaster.Server.Dto;
using EventMaster.Server.Entities;
using EventMaster.Server.UnitOfWork;

namespace EventMaster.Server.UseCases
{
    public class UpdateEventUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEventUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Event> Execute(int eventId, EventDto eventDto)
        {
            var existingEvent = await _unitOfWork.EventRepository.GetEventByIdAsync(eventId);
            if (existingEvent == null)
            {
                throw new KeyNotFoundException("Event not found");
            }

            //_mapper.Map(eventDto, existingEvent);

            existingEvent.Name = eventDto.Name;
            existingEvent.Description = eventDto.Description;
            existingEvent.Date = eventDto.Date;
            existingEvent.Place = eventDto.Place;
            existingEvent.Type = eventDto.Type;
            existingEvent.MaxMemberCount = eventDto.MaxMemberCount;
            existingEvent.ImagePath = eventDto.ImagePath;

            await _unitOfWork.SaveAsync();
            return existingEvent;
        }
    }
}
