using Microsoft.AspNetCore.Mvc;
using EventMaster.Server.Services.Entities;

namespace EventMaster.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly EventService _eventService;

        public UserController(UserService userService, EventService eventService)
        {
            _userService = userService;
            _eventService = eventService;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }
            return Ok(user);
        }

        [HttpGet("{email}/events")]
        public async Task<IActionResult> GetRegisteredEvents(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);

                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                if (user.RegisteredEvents == null || !user.RegisteredEvents.Any())
                {
                    return Ok(new List<Event>());
                }

                return Ok(user.RegisteredEvents);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching registered events for user {email}: {ex.Message}");
                return StatusCode(500, new { Message = "An internal server error occurred. Please try again later." });
            }
        }

        public record UnregisterUserDto(string Email);

        [HttpDelete("{eventId:int}/unregister")]
        public async Task<IActionResult> UnregisterUserFromEventAsync(int eventId, [FromBody] UnregisterUserDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest(new { Message = "Email is required" });
            }

            var userEmail = dto.Email;

            // Получаем событие и пользователя из сервисов
            var eventItem = await _eventService.GetEventByIdAsync(eventId);
            var user = await _userService.GetUserByEmailAsync(userEmail);

            if (eventItem == null || user == null)
            {
                return NotFound(new { Message = "Event or user not found" });
            }

            // Проверяем, зарегистрирован ли пользователь на событие
            if (!eventItem.Users.Any(u => u.Email == userEmail))
            {
                return BadRequest(new { Message = "User is not registered for this event" });
            }

            // Удаляем пользователя из события и событие из зарегистрированных пользователя
            var unregistered = await _eventService.UnregisterUserFromEventAsync(eventItem, user);

            if (!unregistered)
            {
                return StatusCode(500, new { Message = "Failed to unregister user from event. Please try again later." });
            }

            return Ok(new { Message = "User successfully unregistered from the event" });
        }
    }
}
