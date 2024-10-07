using EventMaster.Server.Data;
using EventMaster.Server.Repositories.Implementation;
using EventMaster.Server.Services.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EventMaster.Tests.Repositories
{
    public class EventRepositoryTests
    {
        private async Task<AppDbContext> GetInMemoryDbContextAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}")
                .Options;

            var context = new AppDbContext(options);
            await context.Database.EnsureCreatedAsync();
            return context;
        }

        [Fact]
        public async Task AddEventAsync_ShouldAddEventToDatabase()
        {
            // Arrange
            var context = await GetInMemoryDbContextAsync();
            var eventRepository = new EventRepository(context);
            var newEvent = new Event
            {
                Name = "Music Festival",
                Date = DateTime.Now.AddDays(10),
                Place = "City Park",
                Type = "Festival",
                MaxMemberCount = 100,
                Description = "testexample"
            };

            // Act
            var addResult = await eventRepository.AddEventAsync(newEvent);

            // Assert
            Assert.True(addResult);
            var addedEvent = await context.Events.FirstOrDefaultAsync(e => e.Name == "Music Festival");
            Assert.NotNull(addedEvent);
            Assert.Equal(newEvent.Name, addedEvent.Name);
            Assert.Equal(newEvent.Place, addedEvent.Place);
        }

        [Fact]
        public async Task GetEventByIdAsync_ShouldReturnCorrectEvent()
        {
            // Arrange
            var context = await GetInMemoryDbContextAsync();
            var eventRepository = new EventRepository(context);
            var newEvent = new Event
            {
                Name = "Art Exhibition",
                Date = DateTime.Now.AddDays(15),
                Place = "Art Gallery",
                Type = "Exhibition",
                MaxMemberCount = 50,
                Description = "testexample"
            };
            await eventRepository.AddEventAsync(newEvent);

            // Act
            var retrievedEvent = await eventRepository.GetEventByIdAsync(newEvent.Id);

            // Assert
            Assert.NotNull(retrievedEvent);
            Assert.Equal(newEvent.Name, retrievedEvent.Name);
            Assert.Equal(newEvent.Place, retrievedEvent.Place);
        }

        [Fact]
        public async Task UpdateEventAsync_ShouldUpdateEventDetails()
        {
            // Arrange
            var context = await GetInMemoryDbContextAsync();
            var eventRepository = new EventRepository(context);
            var newEvent = new Event
            {
                Name = "Book Fair",
                Date = DateTime.Now.AddDays(20),
                Place = "Convention Center",
                Type = "Fair",
                MaxMemberCount = 200,
                Description = "testexample"
            };
            await eventRepository.AddEventAsync(newEvent);

            // Act
            newEvent.Place = "Downtown Plaza";
            var updateResult = await eventRepository.UpdateEventAsync(newEvent);

            // Assert
            Assert.True(updateResult);
            var updatedEvent = await eventRepository.GetEventByIdAsync(newEvent.Id);
            Assert.NotNull(updatedEvent);
            Assert.Equal("Downtown Plaza", updatedEvent.Place);
        }

        [Fact]
        public async Task DeleteEventAsync_ShouldRemoveEventFromDatabase()
        {
            // Arrange
            var context = await GetInMemoryDbContextAsync();
            var eventRepository = new EventRepository(context);
            var newEvent = new Event
            {
                Name = "Cooking Class",
                Date = DateTime.Now.AddDays(5),
                Place = "Community Center",
                Type = "Class",
                MaxMemberCount = 20,
                Description = "testexample"
            };
            await eventRepository.AddEventAsync(newEvent);

            // Act
            var deleteResult = await eventRepository.DeleteEventAsync(newEvent.Id);

            // Assert
            Assert.True(deleteResult);
            var deletedEvent = await eventRepository.GetEventByIdAsync(newEvent.Id);
            Assert.Null(deletedEvent);
        }

        [Fact]
        public async Task RegisterUserToEventAsync_ShouldAddUserToEvent()
        {
            // Arrange
            var context = await GetInMemoryDbContextAsync();
            var eventRepository = new EventRepository(context);
            var newEvent = new Event
            {
                Name = "Yoga Session",
                Date = DateTime.Now.AddDays(7),
                Place = "Yoga Studio",
                Type = "Session",
                MaxMemberCount = 15,
                Users = new List<User>(),
                Description = "testexample"
            };
            var user = new User
            {
                FirstName = "Emma",
                LastName = "Wilson",
                Email = "emma.wilson@example.com",
                Password = "password123",
                Role = "User",
                RegisteredEvents = new List<Event>()
            };
            await eventRepository.AddEventAsync(newEvent);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            var registerResult = await eventRepository.RegisterUserToEventAsync(newEvent, user);

            // Assert
            Assert.True(registerResult);
            var registeredEvent = await eventRepository.GetEventByIdAsync(newEvent.Id);
            Assert.Contains(registeredEvent.Users, u => u.Email == "emma.wilson@example.com");
        }

        [Fact]
        public async Task UnregisterUserFromEventAsync_ShouldRemoveUserFromEvent()
        {
            // Arrange
            var context = await GetInMemoryDbContextAsync();
            var eventRepository = new EventRepository(context);
            var newEvent = new Event
            {
                Name = "Dance Workshop",
                Date = DateTime.Now.AddDays(3),
                Place = "Dance Hall",
                Type = "Workshop",
                MaxMemberCount = 30,
                Users = new List<User>(),
                Description = "testexample"
            };
            var user = new User
            {
                FirstName = "Liam",
                LastName = "Brown",
                Email = "liam.brown@example.com",
                Password = "password123",
                Role = "User",
                RegisteredEvents = new List<Event>()
            };
            await eventRepository.AddEventAsync(newEvent);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            await eventRepository.RegisterUserToEventAsync(newEvent, user);

            // Act
            var unregisterResult = await eventRepository.UnregisterUserFromEventAsync(newEvent, user);

            // Assert
            Assert.True(unregisterResult);
            var updatedEvent = await eventRepository.GetEventByIdAsync(newEvent.Id);
            Assert.DoesNotContain(updatedEvent.Users, u => u.Email == "liam.brown@example.com");
        }
    }
}