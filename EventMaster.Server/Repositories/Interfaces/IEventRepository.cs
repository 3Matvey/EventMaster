using EventMaster.Server.Entities;

namespace EventMaster.Server.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task AddEventAsync(Event newEvent);
        Task<Event?> GetEventByIdAsync(int id);
        Task<List<Event>> GetAllEventsAsync();
        Task DeleteEventAsync(int id);
        Task UpdateEventAsync(Event eventItem);
        Task UpdateEventImagePathAsync(int eventId, string imagePath);
        Task UploadImageFileAsync(Event eventItem, IFormFile imageFile);
        Task UploadImageFromUrlAsync(Event eventItem, string imageUrl);
    }

}
