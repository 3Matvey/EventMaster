using EventMaster.Domain.Entities;
using EventMaster.Infrastructure.Data;
using EventMaster.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventMaster.Tests.Repositories
{
    public class EventRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly EventRepository _eventRepository;

        public EventRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();

            _eventRepository = new EventRepository(_context);
        }

        [Fact]
        public async Task AddEventAsync_ShouldAddEventSuccessfully()
        {
            // Arrange
            var newEvent = new Event
            {
                Id = 1,
                Name = "Test Event",
                Description = "Test Description",
                Date = DateTime.UtcNow.AddDays(1),
                Type = "Conference",
                Place = "New York",
                ImagePath = null
            };

            // Act
            await _eventRepository.AddEventAsync(newEvent);
            await _context.SaveChangesAsync();

            // Assert
            var eventInDb = await _context.Events.FindAsync(1);
            Assert.NotNull(eventInDb);
            Assert.Equal("Test Event", eventInDb.Name);
            Assert.Equal("Conference", eventInDb.Type);
            Assert.Equal("New York", eventInDb.Place);
        }

        [Fact]
        public async Task GetEventByIdAsync_ShouldReturnEvent_WhenEventExists()
        {
            // Arrange
            var existingEvent = new Event
            {
                Id = 2,
                Name = "Existing Event",
                Description = "Test Description",
                Date = DateTime.UtcNow.AddDays(2),
                Type = "Workshop",
                Place = "Los Angeles",
                ImagePath = null
            };
            await _context.Events.AddAsync(existingEvent);
            await _context.SaveChangesAsync();

            // Act
            var result = await _eventRepository.GetEventByIdAsync(2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Existing Event", result.Name);
            Assert.Equal("Workshop", result.Type);
            Assert.Equal("Los Angeles", result.Place);
        }

        [Fact]
        public async Task GetEventByIdAsync_ShouldReturnNull_WhenEventDoesNotExist()
        {
            // Act
            var result = await _eventRepository.GetEventByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
