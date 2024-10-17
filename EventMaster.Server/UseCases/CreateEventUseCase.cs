using AutoMapper;
using EventMaster.Server.Dto;
using EventMaster.Server.Entities;
using EventMaster.Server.UnitOfWork;

namespace EventMaster.Server.UseCases
{
    public class CreateEventUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateEventUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Event> Execute(EventDto eventDto)
        {
            ArgumentNullException.ThrowIfNull(eventDto);

            var newEvent = new Event
            {
                Name = eventDto.Name,
                Description = eventDto.Description,
                Date = eventDto.Date,
                Place = eventDto.Place,
                Type = eventDto.Type,
                MaxMemberCount = eventDto.MaxMemberCount,
                ImagePath = eventDto.ImagePath
            };

            await _unitOfWork.EventRepository.AddEventAsync(newEvent);
            await _unitOfWork.SaveAsync();
            return newEvent;
        }
    }
}
