using Microsoft.AspNetCore.Mvc;
using EventMaster.Application.UseCases;
using EventMaster.Application.ResultPattern;
using Microsoft.AspNetCore.Authorization;
using EventMaster.Application.DTOs;

namespace EventMaster.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(
        GetUserByIdUseCase getUserByIdUseCase,
        GetRegisteredEventsUseCase getRegisteredEventsUseCase,
        UnregisterUserFromEventUseCase unregisterUserFromEventUseCase) 
            : BaseController
    {
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await getUserByIdUseCase.Execute(id);

            return result.Match(
                onSuccess: Ok,
                onFailure: Problem
            );
        }

        [HttpGet("{email}/events")]
        public async Task<IActionResult> GetRegisteredEvents(string email)
        {
            var result = await getRegisteredEventsUseCase.Execute(email);

            return result.Match(
                onSuccess: Ok,
                onFailure: Problem
            );
        }

        [HttpDelete("{eventId:int}/unregister")]
        public async Task<IActionResult> UnregisterUserFromEventAsync(int eventId, [FromBody] UnregisterUserDto dto)
        {
            var result = await unregisterUserFromEventUseCase.Execute(eventId, dto.Email);

            return result.Match(
                onSuccess: () => Ok(new { Message = "User successfully unregistered from the event" }),
                onFailure: Problem
            );
        }
    }
}
