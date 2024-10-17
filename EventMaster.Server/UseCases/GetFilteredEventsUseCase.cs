using AutoMapper;
using EventMaster.Server.Dto;
using EventMaster.Server.Entities;
using EventMaster.Server.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace EventMaster.Server.UseCases
{
    public class GetFilteredEventsUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetFilteredEventsUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<EventDto>> Execute(string? name, DateTime? date, string? type, string? place, int pageNumber, int pageSize)
        {
            var events = await _unitOfWork.EventRepository.GetAllEventsAsync();
            var query = events.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase));
            }

            if (date.HasValue)
            {
                query = query.Where(e => e.Date.Date == date.Value.Date);
            }

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(e => e.Type.Contains(type, StringComparison.CurrentCultureIgnoreCase));
            }

            if (!string.IsNullOrEmpty(place))
            {
                query = query.Where(e => e.Place.Contains(place, StringComparison.CurrentCultureIgnoreCase));
            }

            query = query.OrderBy(e => e.Date);
            var filteredEvents = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return _mapper.Map<List<EventDto>>(filteredEvents);
        }
    }
}
