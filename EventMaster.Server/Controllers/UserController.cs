using Microsoft.AspNetCore.Mvc;
using EventMaster.Server.Dto;
using EventMaster.Server.UseCases;
using EventMaster.Server.Entities;

namespace EventMaster.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly GetUserByIdUseCase _getUserByIdUseCase;
        private readonly GetRegisteredEventsUseCase _getRegisteredEventsUseCase;
        private readonly UnregisterUserFromEventUseCase _unregisterUserFromEventUseCase;

        public UserController(
            GetUserByIdUseCase getUserByIdUseCase,
            GetRegisteredEventsUseCase getRegisteredEventsUseCase,
            UnregisterUserFromEventUseCase unregisterUserFromEventUseCase)
        {
            _getUserByIdUseCase = getUserByIdUseCase;
            _getRegisteredEventsUseCase = getRegisteredEventsUseCase;
            _unregisterUserFromEventUseCase = unregisterUserFromEventUseCase;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _getUserByIdUseCase.Execute(id);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }
            return Ok(user);
        }

        [HttpGet("{email}/events")]
        public async Task<IActionResult> GetRegisteredEvents(string email)
        {
            var registeredEvents = await _getRegisteredEventsUseCase.Execute(email);
            return Ok(registeredEvents);
        }

        [HttpDelete("{eventId:int}/unregister")]
        public async Task<IActionResult> UnregisterUserFromEventAsync(int eventId, [FromBody] UnregisterUserDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest(new { Message = "Email is required" });
            }

            var unregistered = await _unregisterUserFromEventUseCase.Execute(eventId, dto.Email);
            if (!unregistered)
            {
                return StatusCode(500, new { Message = "Failed to unregister user from event. Please try again later." });
            }

            return Ok(new { Message = "User successfully unregistered from the event" });
        }
    }
}
