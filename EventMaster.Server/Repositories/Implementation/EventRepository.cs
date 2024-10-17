using EventMaster.Server.Data;
using EventMaster.Server.Repositories.Interfaces;
//using static EventMaster.Server.Services.RemoteDataFetcher;
using Microsoft.EntityFrameworkCore;
using EventMaster.Server.Entities;

namespace EventMaster.Server.Repositories.Implementation
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddEventAsync(Event newEvent)
        {
            await _context.Events.AddAsync(newEvent);
        }

        public async Task<Event?> GetEventByIdAsync(int id)
        {
            return await _context.Events.Include(e => e.Users).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task DeleteEventAsync(int id)
        {
            var eventItem = await GetEventByIdAsync(id);
            if (eventItem != null)
            {
                _context.Events.Remove(eventItem);
            }
        }

        public async Task UpdateEventAsync(Event eventItem)
        {
            _context.Events.Update(eventItem);
        }

        public async Task UpdateEventImagePathAsync(int eventId, string imagePath)
        {
            var eventItem = await GetEventByIdAsync(eventId);
            if (eventItem != null)
            {
                eventItem.ImagePath = imagePath;
                _context.Events.Update(eventItem);
            }
        }

        public async Task UploadImageFileAsync(Event eventItem, IFormFile imageFile)
        {
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            string fileName = $"{eventItem.Id}_{Path.GetFileName(imageFile.FileName)}";
            string filePath = Path.Combine(uploadPath, fileName);

            if (!string.IsNullOrEmpty(eventItem.ImagePath) && eventItem.ImagePath.StartsWith("/images/"))
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", eventItem.ImagePath.TrimStart('/'));
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            eventItem.ImagePath = $"/images/{fileName}";
            _context.Events.Update(eventItem);
        }

        public async Task UploadImageFromUrlAsync(Event eventItem, string imageUrl)
        {
            if (Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                var imageBytes = await new HttpClient().GetByteArrayAsync(imageUrl);

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string fileName = $"{eventItem.Id}.jpg";
                string filePath = Path.Combine(uploadPath, fileName);

                await File.WriteAllBytesAsync(filePath, imageBytes);

                eventItem.ImagePath = $"/images/{fileName}";
                _context.Events.Update(eventItem);
            }
        }
    }
}
