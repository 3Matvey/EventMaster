using Microsoft.AspNetCore.Mvc;
using EventMaster.Server.Services;
using EventMaster.Server.Services.Entities;
using System.ComponentModel.DataAnnotations;

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
        public ActionResult<IEnumerable<Event>> GetEvents()
        {
            var events = _eventService.GetAllEvents();
            return Ok(events);
        }

        // Получение события по имени
        [HttpGet("{name}")]
        public ActionResult<Event> GetEvent(string name)
        {
            var eventItem = _eventService.GetEventByName(name);
            if (eventItem == null)
            {
                return NotFound();
            }
            return Ok(eventItem);
        }

        // Создание нового события
        [HttpPost]
        public ActionResult<Event> CreateEvent(Event newEvent)
        {
            _eventService.AddEvent(newEvent);
            return CreatedAtAction(nameof(GetEvent), new { name = newEvent.Name }, newEvent);
        }

        // Обновление события
        [HttpPut("{name}")]
        public IActionResult UpdateEvent(string name, Event updatedEvent)
        {
            bool success = _eventService.UpdateEvent(name, updatedEvent);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        // Удаление события
        [HttpDelete("{name}")]
        public IActionResult DeleteEvent(string name)
        {
            bool success = _eventService.DeleteEvent(name);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        public class RegisterToEventDto
        {
            public string Email { get; set; }
        }

        [HttpPost("{eventName}/register_to_event")]
        public IActionResult RegisterUserToEvent(string eventName, [FromBody] RegisterToEventDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest(new { Message = "Email is required" });
            }

            var userEmail = dto.Email;
            var eventItem = _eventService.GetEventByName(eventName);
            var user = _userService.GetUserByEmail(userEmail);

            if (eventItem == null || user == null)
            {
                return NotFound(new { Message = "Event or user not found" });
            }

            if (eventItem.Users.Any(u => u.Email == userEmail))
            {
                return BadRequest(new { Message = "User is already registered for this event" });
            }

            if (eventItem.Users.Count >= eventItem.MaxMemberCount)
            {
                return BadRequest(new { Message = "The event has reached its maximum member capacity" });
            }

            eventItem.Users.Add(user);
            user.RegisteredEvents.Add(eventItem);

            return Ok(new { Message = "User successfully registered for the event" });
        }


        [HttpGet("user/{email}/events")]
        public IActionResult GetRegisteredEvents(string email)
        {
            try
            {
                // Проверяем, есть ли пользователь с данным email
                var user = _userService.GetUserByEmail(email);

                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                if (user.RegisteredEvents == null)
                {
                    // Если пользователь существует, но зарегистрированных событий нет
                    return Ok(new List<Event>()); // Возвращаем пустой список вместо ошибки
                }

                return Ok(user.RegisteredEvents);
            }
            catch (Exception ex)
            {
                // Логируем исключение для анализа
                Console.WriteLine($"Error fetching registered events for user {email}: {ex.Message}");
                return StatusCode(500, new { Message = "An internal server error occurred. Please try again later." });
            }
        }


        // Получение пользователей по событию
        [HttpGet("{name}/users")]
        public ActionResult<IEnumerable<User>> GetEventUsers(string name)
        {
            var eventItem = _eventService.GetEventByName(name);
            if (eventItem == null)
            {
                return NotFound();
            }

            return Ok(eventItem.Users);
        }

        [HttpGet("paged")]
        public IActionResult GetPagedEvents(int pageNumber = 1, int pageSize = 10)
        {
            var pagedEvents = _eventService.GetAllEvents()
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList();
            return Ok(pagedEvents);
        }

        [HttpGet("search")]
        public IActionResult SearchEvents(string? name, DateTime? date)
        {
            var events = _eventService.GetAllEvents();

            if (!string.IsNullOrEmpty(name))
            {
                events = events.Where(e => e.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (date.HasValue)
            {
                events = events.Where(e => e.Date.Date == date.Value.Date).ToList();
            }

            return Ok(events);
        }

        [HttpGet("filter")]
        public IActionResult FilterEvents(string? category, string? place)
        {
            var events = _eventService.GetAllEvents();

            if (!string.IsNullOrEmpty(category))
            {
                events = events.Where(e => e.Type == category).ToList();
            }

            if (!string.IsNullOrEmpty(place))
            {
                events = events.Where(e => e.Place.Contains(place, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return Ok(events);
        }
    }
}
