using EventMaster.Server.Data;
using EventMaster.Server.Repositories.Interfaces;
using EventMaster.Server.Services.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventMaster.Server.Repositories.Implementation
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddEventAsync(Event newEvent)
        {
            await _context.Events.AddAsync(newEvent);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Event?> GetEventByIdAsync(int id)
        {
            return await _context.Events.Include(e => e.Users).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            return await _context.Events.Include(e => e.Users).ToListAsync();
        }

        public async Task<bool> UpdateEventAsync(Event updatedEvent)
        {
            var existingEvent = await GetEventByIdAsync(updatedEvent.Id);
            if (existingEvent == null)
                return false;

            _context.Entry(existingEvent).CurrentValues.SetValues(updatedEvent);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var eventItem = await GetEventByIdAsync(id);
            if (eventItem == null)
                return false;

            _context.Events.Remove(eventItem);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RegisterUserToEventAsync(Event eventItem, User user)
        {
            if (eventItem.Users.Any(u => u.Id == user.Id) || eventItem.Users.Count >= eventItem.MaxMemberCount)
                return false;

            eventItem.Users.Add(user);
            user.RegisteredEvents.Add(eventItem);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UnregisterUserFromEventAsync(Event eventItem, User user)
        {
            if (!eventItem.Users.Any(u => u.Id == user.Id))
                return false;

            eventItem.Users.Remove(user);
            user.RegisteredEvents.Remove(eventItem);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateEventImagePathAsync(int eventId, string imagePath)
        {
            var eventItem = await GetEventByIdAsync(eventId);
            if (eventItem == null)
                return false;

            eventItem.ImagePath = imagePath;
            _context.Events.Update(eventItem);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Event>> GetFilteredEventsAsync(string? name, DateTime? date, string? type, string? place, int pageNumber, int pageSize)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.Name.Contains(name));
            }

            if (date.HasValue)
            {
                query = query.Where(e => e.Date.Date == date.Value.Date);
            }

            if (!string.IsNullOrEmpty(type))
            {
               query = query.Where(e => e.Type.Contains(type));
            }

            if (!string.IsNullOrEmpty(place))
            {
                query = query.Where(e => e.Place.Contains(place));
            }

            query = query.OrderBy(e => e.Date);
            

            return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<bool> UploadImageFileAsync(Event eventItem, IFormFile imageFile)
        {
            try
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

                return await UpdateEventImagePathAsync(eventItem.Id, $"/images/{fileName}");
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UploadImageFromUrlAsync(Event eventItem, string imageUrl)
        {
            if (Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                try
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        string fileName = $"{eventItem.Id}.jpg";
                        string filePath = Path.Combine(uploadPath, fileName);

                        await File.WriteAllBytesAsync(filePath, imageBytes);

                        return await UpdateEventImagePathAsync(eventItem.Id, $"/images/{fileName}");
                    }
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                throw new ArgumentException("Invalid image URL provided.");
            }
        }
    }
}
