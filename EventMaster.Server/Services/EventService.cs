using EventMaster.Server.Services.Entities;
using EventMaster.Server.UnitOfWork;

public class EventService
{
    private readonly IUnitOfWork _unitOfWork;

    public EventService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> AddEventAsync(Event newEvent)
    {
        var success = await _unitOfWork.EventRepository.AddEventAsync(newEvent);
        if (success)
        {
            await _unitOfWork.SaveAsync();
        }
        return success;
    }

    public async Task<Event?> GetEventByIdAsync(int id)
    {
        return await _unitOfWork.EventRepository.GetEventByIdAsync(id);
    }

    public async Task<List<Event>> GetAllEventsAsync()
    {
        return await _unitOfWork.EventRepository.GetAllEventsAsync();
    }

    public async Task<bool> UpdateEventAsync(Event updatedEvent)
    {
        var success = await _unitOfWork.EventRepository.UpdateEventAsync(updatedEvent);
        if (success)
        {
            await _unitOfWork.SaveAsync();
        }
        return success;
    }

    public async Task<bool> DeleteEventAsync(int id)
    {
        var success = await _unitOfWork.EventRepository.DeleteEventAsync(id);
        if (success)
        {
            await _unitOfWork.SaveAsync();
        }
        return success;
    }

    public async Task<bool> RegisterUserToEventAsync(Event eventItem, User user)
    {
        var success = await _unitOfWork.EventRepository.RegisterUserToEventAsync(eventItem, user);
        if (success)
        {
            await _unitOfWork.SaveAsync();
        }
        return success;
    }

    public async Task<bool> UnregisterUserFromEventAsync(Event eventItem, User user)
    {
        var success = await _unitOfWork.EventRepository.UnregisterUserFromEventAsync(eventItem, user);
        if (success)
        {
            await _unitOfWork.SaveAsync();
        }
        return success;
    }

    public async Task<bool> UpdateEventImagePathAsync(int eventId, string imagePath)
    {
        var success = await _unitOfWork.EventRepository.UpdateEventImagePathAsync(eventId, imagePath);
        if (success)
        {
            await _unitOfWork.SaveAsync();
        }
        return success;
    }

    public async Task<List<Event>> GetFilteredEventsAsync(string? name, DateTime? date, string? type, string? place, int pageNumber, int pageSize)
    {
        return await _unitOfWork.EventRepository.GetFilteredEventsAsync(name, date, type, place, pageNumber, pageSize);
    }

    public async Task<bool> UploadImageAsync(Event eventItem, IFormFile imageFile)
    {
        var success = await _unitOfWork.EventRepository.UploadImageFileAsync(eventItem, imageFile);
        if (success)
        {
            await _unitOfWork.SaveAsync();
        }
        return success;
    }

    public async Task<bool> UploadImageUrlAsync(Event eventItem, string imageUrl)
    {
        var success = await _unitOfWork.EventRepository.UploadImageFromUrlAsync(eventItem, imageUrl);
        if (success)
        {
            await _unitOfWork.SaveAsync();
        }
        return success;
    }
}
