using Moq;
using AutoMapper;
using EventMaster.Server.UnitOfWork;
using EventMaster.Server.Entities;
using EventMaster.Server.Dto;
using EventMaster.Server.UseCases;
using Microsoft.Extensions.Options;

namespace EventMaster.Tests.UseCasesTests
{
    public class UseCaseTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;

        public UseCaseTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetFilteredEventsUseCase_ShouldReturnFilteredEvents()
        {
            // Arrange
            var useCase = new GetFilteredEventsUseCase(_unitOfWorkMock.Object, _mapperMock.Object);
            var eventList = new List<Event> { new Event { Id = 1, Name = "Test Event" } };
            _unitOfWorkMock.Setup(u => u.EventRepository.GetAllEventsAsync()).ReturnsAsync(eventList);
            _mapperMock.Setup(m => m.Map<List<EventDto>>(It.IsAny<List<Event>>())).Returns(new List<EventDto> { new EventDto { Id = 1, Name = "Test Event" } });

            // Act
            var result = await useCase.Execute("Test", null, null, null, 1, 10);

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Event", result[0].Name);
        }

        [Fact]
        public async Task GetUserByIdUseCase_ShouldReturnUserDto_WhenUserExists()
        {
            // Arrange
            var useCase = new GetUserByIdUseCase(_unitOfWorkMock.Object, _mapperMock.Object);
            var user = new User { Id = 1, Email = "test@example.com" };
            _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto { Id = 1, Email = "test@example.com" });

            // Act
            var result = await useCase.Execute(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task RegisterUserToEventUseCase_ShouldRegisterUserToEvent()
        {
            // Arrange
            var useCase = new RegisterUserToEventUseCase(_unitOfWorkMock.Object);
            var eventItem = new Event { Id = 1, Name = "Test Event", MaxMemberCount = 5, Users = new List<User>() };
            var user = new User { Id = 1, Email = "test@example.com" };

            _unitOfWorkMock.Setup(u => u.EventRepository.GetEventByIdAsync(It.IsAny<int>())).ReturnsAsync(eventItem);
            _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.EventRepository.UpdateEventAsync(eventItem)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveAsync());//.Returns((Task<int>)Task.CompletedTask);

            // Act
            var result = await useCase.Execute(1, "test@example.com");

            // Assert
            Assert.True(result);
            Assert.Contains(eventItem.Users, u => u.Email == "test@example.com");
        }

        [Fact]
        public async Task UpdateEventUseCase_ShouldUpdateEvent_WhenEventExists()
        {
            // Arrange
            var useCase = new UpdateEventUseCase(_unitOfWorkMock.Object);
            var eventDto = new EventDto { Id = 1, Name = "Updated Event" };
            var existingEvent = new Event { Id = 1, Name = "Old Event" };

            _unitOfWorkMock.Setup(u => u.EventRepository.GetEventByIdAsync(It.IsAny<int>())).ReturnsAsync(existingEvent);

            // Act
            var result = await useCase.Execute(1, eventDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Event", result.Name);
        }

        [Fact]
        public async Task CreateEventUseCase_ShouldCreateNewEvent()
        {
            // Arrange
            var useCase = new CreateEventUseCase(_unitOfWorkMock.Object);
            var eventDto = new EventDto { Name = "New Event" };
            var newEvent = new Event { Id = 1, Name = "New Event" };

            _unitOfWorkMock.Setup(u => u.EventRepository.AddEventAsync(newEvent)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveAsync());

            // Act
            var result = await useCase.Execute(eventDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Event", result.Name);
        }

        [Fact]
        public async Task GetEventDetailsUseCase_ShouldReturnEventDetails_WhenEventExists()
        {
            // Arrange
            var useCase = new GetEventDetailsUseCase(_unitOfWorkMock.Object);
            var eventItem = new Event { Id = 1, Name = "Event Details Test" };

            _unitOfWorkMock.Setup(u => u.EventRepository.GetEventByIdAsync(It.IsAny<int>())).ReturnsAsync(eventItem);

            // Act
            var result = await useCase.Execute(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Event Details Test", result.Name);
        }

        [Fact]
        public async Task DeleteEventUseCase_ShouldDeleteEvent_WhenEventExists()
        {
            // Arrange
            var useCase = new DeleteEventUseCase(_unitOfWorkMock.Object);
            var eventItem = new Event { Id = 1, Name = "Event to Delete" };

            _unitOfWorkMock.Setup(u => u.EventRepository.GetEventByIdAsync(It.IsAny<int>())).ReturnsAsync(eventItem);
            _unitOfWorkMock.Setup(u => u.EventRepository.DeleteEventAsync(eventItem)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveAsync());

            // Act
            await useCase.Execute(1);

            // Assert
            _unitOfWorkMock.Verify(u => u.EventRepository.DeleteEventAsync(eventItem), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
