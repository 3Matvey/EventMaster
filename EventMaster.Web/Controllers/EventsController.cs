using Microsoft.AspNetCore.Mvc;
using EventMaster.Application.UseCases;
using EventMaster.Application.ResultPattern;
using Microsoft.AspNetCore.Authorization;
using EventMaster.Application.DTOs;

namespace EventMaster.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController(
        GetAllEventsUseCase getAllEventsUseCase,
        GetEventDetailsUseCase getEventDetailsUseCase,
        CreateEventUseCase createEventUseCase,
        UpdateEventUseCase updateEventUseCase,
        DeleteEventUseCase deleteEventUseCase,
        RegisterUserToEventUseCase registerUserToEventUseCase,
        GetFilteredEventsUseCase getFilteredEventsUseCase,
        UploadEventImageUseCase uploadEventImageUseCase) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var result = await getAllEventsUseCase.Execute();

            return result.Match(
                onSuccess: Ok,
                onFailure: Problem
            );
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var result = await getEventDetailsUseCase.Execute(id);

            return result.Match(
                onSuccess: Ok,
                onFailure: Problem
            );
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto newEvent)
        {
            var result = await createEventUseCase.Execute(newEvent);

            return result.Match(
                onSuccess: createdEvent => CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent),
                onFailure: Problem
            );
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventDto updatedEvent)
        {
            var result = await updateEventUseCase.Execute(id, updatedEvent);

            return result.Match(
                onSuccess: Ok,
                onFailure: Problem
            );
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var result = await deleteEventUseCase.Execute(id);

            return result.Match(
                onSuccess: NoContent,
                onFailure: Problem
            );
        }

        [HttpPost("{id:int}/register")]
        public async Task<IActionResult> RegisterUserToEvent(int id, [FromBody] RegisterToEventDto dto)
        {
            var result = await registerUserToEventUseCase.Execute(id, dto);

            return result.Match(
                onSuccess: () => Ok(new { Message = "User successfully registered for the event" }),
                onFailure: Problem
            );
        }

        [HttpGet("{id:int}/users")]
        public async Task<IActionResult> GetEventUsers(int id)
        {
            var result = await getEventDetailsUseCase.Execute(id);

            return result.Match(
                onSuccess: Ok,
                onFailure: Problem
            );
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredEvents(
            [FromQuery] string? name,
            [FromQuery] DateTime? date,
            [FromQuery] string? type,
            [FromQuery] string? place,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize) //updated
        {
            var result = await getFilteredEventsUseCase.Execute(name, date, type, place, pageNumber, pageSize);

            return result.Match(
                onSuccess: Ok,
                onFailure: Problem
            );
        }

        [HttpPost("{id:int}/upload-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImageOrUrl(int id, [FromForm] UploadImageOrUrlRequest request)
        {
            var result = await uploadEventImageUseCase.Execute(id, request);

            return result.Match(
                onSuccess: imagePath => Ok(new { Message = "Image successfully uploaded.", ImagePath = imagePath }),
                onFailure: Problem
            );
        }
    }
}
