using EventMaster.Server.Entities;

namespace EventMaster.Server.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task AddEventAsync(Event newEvent);
        Task<Event?> GetEventByIdAsync(int id);
        Task<List<Event>> GetAllEventsAsync();
        Task DeleteEventAsync(Event eventItem);
        Task UpdateEventAsync(Event eventItem);
        Task UpdateEventImagePathAsync(Event eventItem, string imagePath);
        Task UploadImageFileAsync(Event eventItem, IFormFile imageFile);
        Task UploadImageFromUrlAsync(Event eventItem, string imageUrl);
    }

}
