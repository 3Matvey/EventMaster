using Microsoft.AspNetCore.Http;

namespace EventMaster.Application.Interfaces.Repositories
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
        Task<List<Event>> GetFilteredEventsAsync(string? name, DateTime? date, string? type, string? place, int pageNumber, int pageSize);
    }
}
