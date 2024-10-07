using Microsoft.AspNetCore.Mvc;
using EventMaster.Server.Services.Entities;

namespace EventMaster.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly EventService _eventService;
        private readonly UserService _userService;

        public EventsController(EventService eventService, UserService userService)
        {
            _eventService = eventService;
            _userService = userService;
        }

        // Получение всех событий
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            try
            {
                var events = await _eventService.GetAllEventsAsync();

                if (events == null || events.Count == 0)
                {
                    return NotFound(new { Message = "No events found" });
                }

                return Ok(events.OrderBy(e => e.Date));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching events: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while fetching the events. Please try again later." });
            }
        }

        // Получение события по ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var eventItem = await _eventService.GetEventByIdAsync(id);
            if (eventItem == null)
            {
                return NotFound(new { Message = "Event not found" });
            }
            return Ok(eventItem);
        }

        // Создание нового события
        [HttpPost("create_event")]
        public async Task<ActionResult<Event>> CreateEvent([FromBody] Event newEvent)
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

            try
            {
                await _eventService.AddEventAsync(newEvent);
                return CreatedAtAction(nameof(GetEvent), new { id = newEvent.Id }, newEvent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating event: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while creating the event." });
            }
        }

        // Обновление события
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event updatedEvent)
        {
            updatedEvent.Id = id;
            var success = await _eventService.UpdateEventAsync(updatedEvent);
            if (!success)
            {
                return NotFound(new { Message = "Event not found" });
            }

            return Ok(updatedEvent);
        }

        // Удаление события
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var success = await _eventService.DeleteEventAsync(id);
            if (!success)
            {
                return NotFound(new { Message = "Event not found" });
            }

            return NoContent();
        }

        // Регистрация пользователя на событие
        public record RegisterToEventDto(string Email);

        [HttpPost("{id:int}/register_to_event")]
        public async Task<IActionResult> RegisterUserToEvent(int id, [FromBody] RegisterToEventDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest(new { Message = "Email is required" });
            }

            var eventItem = await _eventService.GetEventByIdAsync(id);
            var user = await _userService.GetUserByEmailAsync(dto.Email);

            if (eventItem == null || user == null)
            {
                return NotFound(new { Message = "Event or user not found" });
            }

            var success = await _eventService.RegisterUserToEventAsync(eventItem, user);
            if (!success)
            {
                return BadRequest(new { Message = "User is already registered for this event or event is full" });
            }

            return Ok(new { Message = "User successfully registered for the event" });
        }

        // Получение пользователей по событию
        [HttpGet("{id:int}/users")]
        public async Task<ActionResult<IEnumerable<User>>> GetEventUsers(int id)
        {
            var eventItem = await _eventService.GetEventByIdAsync(id);
            if (eventItem == null)
            {
                return NotFound(new { Message = "Event not found" });
            }

            return Ok(eventItem.Users);
        }

        // Получение фильтрованных событий
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Event>>> GetFilteredEvents(
            [FromQuery] string? name,
            [FromQuery] DateTime? date,
            [FromQuery] string? type,
            [FromQuery] string? place,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var events = await _eventService.GetFilteredEventsAsync(name, date, type, place, pageNumber, pageSize);
            return Ok(events);
        }

        // Загрузка изображения для события
        [HttpPost("{id:int}/upload-image")]
        public async Task<IActionResult> UploadImageOrUrl(int id, [FromForm] IFormFile? imageFile, [FromForm] string? imageUrl)
        {
            var eventItem = await _eventService.GetEventByIdAsync(id);
            if (eventItem == null)
            {
                return NotFound(new { Message = "Event not found." });
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                return await UploadImageFile(eventItem, imageFile);
            }
            else if (!string.IsNullOrEmpty(imageUrl))
            {
                return await UploadImageUrl(eventItem, imageUrl);
            }
            else
            {
                return BadRequest(new { Message = "No file or URL was provided." });
            }
        }

        private async Task<IActionResult> UploadImageFile(Event eventItem, IFormFile imageFile)
        {
            try
            {
                await _eventService.UploadImageAsync(eventItem, imageFile);
                return Ok(new { Message = "Image uploaded successfully.", ImagePath = eventItem.ImagePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to upload image.", Error = ex.Message });
            }
        }

        private async Task<IActionResult> UploadImageUrl(Event eventItem, string imageUrl)
        {
            try
            {
                await _eventService.UploadImageUrlAsync(eventItem, imageUrl);
                return Ok(new { Message = "Image URL successfully added to the event.", ImagePath = eventItem.ImagePath });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Failed to download image from URL.", Error = ex.Message });
            }
        }

    }
}
