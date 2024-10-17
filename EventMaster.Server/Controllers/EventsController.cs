using Microsoft.AspNetCore.Mvc;
using EventMaster.Server.UseCases;
using EventMaster.Server.Dto;
using EventMaster.Server.Entities;
//using EventMaster.Server.WebApi.Dto;

namespace EventMaster.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly GetAllEventsUseCase _getAllEventsUseCase;
        private readonly GetEventDetailsUseCase _getEventDetailsUseCase;
        private readonly CreateEventUseCase _createEventUseCase;
        private readonly UpdateEventUseCase _updateEventUseCase;
        private readonly DeleteEventUseCase _deleteEventUseCase;
        private readonly RegisterUserToEventUseCase _registerUserToEventUseCase;
        private readonly GetFilteredEventsUseCase _getFilteredEventsUseCase;
        private readonly UploadEventImageUseCase _uploadEventImageUseCase;

        public EventsController(
            GetAllEventsUseCase getAllEventsUseCase,
            GetEventDetailsUseCase getEventDetailsUseCase,
            CreateEventUseCase createEventUseCase,
            UpdateEventUseCase updateEventUseCase,
            DeleteEventUseCase deleteEventUseCase,
            RegisterUserToEventUseCase registerUserToEventUseCase,
            GetFilteredEventsUseCase getFilteredEventsUseCase,
            UploadEventImageUseCase uploadEventImageUseCase)
        {
            _getAllEventsUseCase = getAllEventsUseCase;
            _getEventDetailsUseCase = getEventDetailsUseCase;
            _createEventUseCase = createEventUseCase;
            _updateEventUseCase = updateEventUseCase;
            _deleteEventUseCase = deleteEventUseCase;
            _registerUserToEventUseCase = registerUserToEventUseCase;
            _getFilteredEventsUseCase = getFilteredEventsUseCase;
            _uploadEventImageUseCase = uploadEventImageUseCase;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            var events = await _getAllEventsUseCase.Execute();
            if (events == null || events.Count == 0)
            {
                return NotFound(new { Message = "No events found" });
            }
            return Ok(events.OrderBy(e => e.Date));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var eventItem = await _getEventDetailsUseCase.Execute(id);
            if (eventItem == null)
            {
                return NotFound(new { Message = "Event not found" });
            }
            return Ok(eventItem);
        }

        [HttpPost("create_event")]
        public async Task<ActionResult<Event>> CreateEvent([FromBody] EventDto newEvent)
        {
            if (newEvent == null ||
                string.IsNullOrEmpty(newEvent.Name) ||
                string.IsNullOrEmpty(newEvent.Description) ||
                string.IsNullOrEmpty(newEvent.Place) ||
                newEvent.Date == default ||
                newEvent.MaxMemberCount <= 0)
            {
                return BadRequest(new { Message = "Invalid event data. Please ensure all fields are filled correctly." });
            }

            var createdEvent = await _createEventUseCase.Execute(newEvent);
            return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventDto updatedEvent)
        {
            var @event = await _updateEventUseCase.Execute(id, updatedEvent);
            if (@event is null)
            {
                return NotFound(new { Message = "Event not found" });
            }

            return Ok(updatedEvent);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var success = await _deleteEventUseCase.Execute(id);
            if (!success)
            {
                return NotFound(new { Message = "Event not found" });
            }

            return NoContent();
        }

        [HttpPost("{id:int}/register_to_event")]
        public async Task<IActionResult> RegisterUserToEvent(int id, [FromBody] RegisterToEventDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest(new { Message = "Email is required" });
            }

            var success = await _registerUserToEventUseCase.Execute(id, dto.Email);
            if (!success)
            {
                return BadRequest(new { Message = "User is already registered for this event or event is full" });
            }

            return Ok(new { Message = "User successfully registered for the event" });
        }

        [HttpGet("{id:int}/users")]
        public async Task<ActionResult<IEnumerable<User>>> GetEventUsers(int id)
        {
            var users = await _getEventDetailsUseCase.Execute(id);
            if (users == null || users.Users == null || !users.Users.Any())
            {
                return NotFound(new { Message = "No users found for this event" });
            }
            return Ok(users.Users);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Event>>> GetFilteredEvents(
            [FromQuery] string? name,
            [FromQuery] DateTime? date,
            [FromQuery] string? type,
            [FromQuery] string? place,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var events = await _getFilteredEventsUseCase.Execute(name, date, type, place, pageNumber, pageSize);
            return Ok(events);
        }

        [HttpPost("{id:int}/upload-image")]
        public async Task<IActionResult> UploadImageOrUrl(int id, [FromForm] IFormFile? imageFile, [FromForm] string? imageUrl)
        {
            var result = await _uploadEventImageUseCase.Execute(id, imageFile, imageUrl);
            if (!result.Success)
            {
                return BadRequest(new { Message = result.Message });
            }
            return Ok(new { Message = result.Message, ImagePath = result.ImagePath });
        }
    }

    public record RegisterToEventDto(string Email);
}
