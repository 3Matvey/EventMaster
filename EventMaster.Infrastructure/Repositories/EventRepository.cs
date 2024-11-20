using Microsoft.EntityFrameworkCore;
using EventMaster.Infrastructure.Data;
using EventMaster.Domain.Entities;
using Microsoft.AspNetCore.Http;
using EventMaster.Application.Interfaces.Repositories;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace EventMaster.Infrastructure.Repositories
{
    public class EventRepository(AppDbContext context) : IEventRepository
    {
        private readonly AppDbContext _context = context;

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

        public async Task DeleteEventAsync(Event eventItem)
        {
            if (eventItem.ImagePath != null /*&& eventItem.ImagePath.StartsWith("/images/")*/)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", eventItem.ImagePath.TrimStart('/'));
                if (File.Exists(path))
                    File.Delete(path);
            }
            await Task.Run(() =>
            {
                _context.Events.Remove(eventItem);
            });
        }

        public async Task<List<Event>> GetFilteredEventsAsync(string? name, DateTime? date, string? type, string? place, int pageNumber, int pageSize)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => EF.Functions.Like(e.Name, $"%{name}%"));
            }

            if (date.HasValue)
            {
                query = query.Where(e => e.Date.Date == date.Value.Date);
            }

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(e => EF.Functions.Like(e.Type, $"%{type}%"));
            }

            if (!string.IsNullOrEmpty(place))
            {
                query = query.Where(e => EF.Functions.Like(e.Place, $"%{place}%"));
            }

            query = query.OrderBy(e => e.Date);

            var filteredEvents = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return filteredEvents;
        }

        public async Task UpdateEventAsync(Event eventItem)
        {
            await Task.Run(() =>
            {
                //_context.Events.Update(eventItem);

                if (eventItem.ImagePath != null)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", eventItem.ImagePath.TrimStart('/'));
                    if (File.Exists(path))
                        File.Delete(path);
                }
                _context.Events.Update(eventItem);
            });
        }

        [Obsolete("temp")]
        public async Task UpdateEventImagePathAsync(Event eventItem, string imagePath)
        {
            await Task.Run(() =>
            {
                if (eventItem.ImagePath != imagePath && eventItem.ImagePath != null)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", eventItem.ImagePath.TrimStart('/'));
                    if (File.Exists(path))
                        File.Delete(path);
                }
                eventItem.ImagePath = imagePath;
                _context.Events.Update(eventItem);
            });
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

            using (var image = await Image.LoadAsync(imageFile.OpenReadStream()))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(800, 600),
                    Mode = ResizeMode.Max
                }));

                await image.SaveAsync(filePath);
            }

            eventItem.ImagePath = $"/images/{fileName}";
            _context.Events.Update(eventItem);
        }

       public async Task UploadImageFromUrlAsync(Event eventItem, string imageUrl)
       {
            ArgumentNullException.ThrowIfNull(eventItem);

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                throw new ArgumentException("Image URL must be provided.", nameof(imageUrl));
            }

            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                throw new ArgumentException("Invalid URL format.", nameof(imageUrl));
            }

            try
            {
                using var httpClient = new HttpClient();

                httpClient.Timeout = TimeSpan.FromSeconds(10);

                var response = await httpClient.GetAsync(imageUrl);

                response.EnsureSuccessStatusCode();

                var contentType = response.Content.Headers.ContentType?.MediaType;

                var validContentTypes = new List<string> { "image/jpeg", "image/png", "image/gif" };
                if (contentType == null || !validContentTypes.Contains(contentType))
                {
                    throw new InvalidOperationException("URL does not point to a supported image format.");
                }

                var extension = contentType switch
                {
                    "image/jpeg" => ".jpg",
                    "image/png" => ".png",
                    "image/gif" => ".gif",
                    _ => ".jpg"
                };

                var imageBytes = await response.Content.ReadAsByteArrayAsync();

                // не более 5 МБ
                const uint limit = 5 * 1024 * 1024;
                if (imageBytes.Length > limit)
                {
                    throw new InvalidOperationException("Image size exceeds the maximum allowed size of 5 MB.");
                }

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string fileName = $"{eventItem.Id}_{Guid.NewGuid()}{extension}";
                string filePath = Path.Combine(uploadPath, fileName);

                if (!string.IsNullOrEmpty(eventItem.ImagePath))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", eventItem.ImagePath.TrimStart('/'));
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                await File.WriteAllBytesAsync(filePath, imageBytes);

                eventItem.ImagePath = $"/images/{fileName}";
                _context.Events.Update(eventItem);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Image not loaded");
            }
        }
    }
}
