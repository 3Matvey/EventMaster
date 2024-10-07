using EventMaster.Server.Services.Entities;

namespace EventMaster.Server.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task<bool> AddEventAsync(Event newEvent);
        Task<Event?> GetEventByIdAsync(int id);
        Task<List<Event>> GetAllEventsAsync();
        Task<bool> UpdateEventAsync(Event updatedEvent);
        Task<bool> DeleteEventAsync(int id);
        Task<bool> RegisterUserToEventAsync(Event eventItem, User user);
        Task<bool> UnregisterUserFromEventAsync(Event eventItem, User user);
        Task<bool> UpdateEventImagePathAsync(int eventId, string imagePath);
        Task<List<Event>> GetFilteredEventsAsync(string? name, DateTime? date, string? type, string? place, int pageNumber, int pageSize);
        Task<bool> UploadImageFileAsync(Event eventItem, IFormFile imageFile);
        Task<bool> UploadImageFromUrlAsync(Event eventItem, string imageUrl);
    }
}
