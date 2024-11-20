using Moq;
using AutoMapper;
using EventMaster.Application.Interfaces.UnitOfWork;
using EventMaster.Application.UseCases;
using EventMaster.Domain.Entities;
using EventMaster.Application.DTOs;

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
            var eventDtoList = new List<EventDto> { new EventDto { Id = 1, Name = "Test Event" } };

            // настройка мок репозитория 
            _unitOfWorkMock.Setup(u => u.EventRepository.GetFilteredEventsAsync(
                "Test Event", 
                null,         
                null,         
                null,         
                1,            
                10            
            )).ReturnsAsync(eventList);

            _mapperMock.Setup(m => m.Map<List<EventDto>>(eventList)).Returns(eventDtoList);

            // Act
            var result = await useCase.Execute("Test Event", null, null, null, 1, 10);

            // Assert
            Assert.True(result.IsSuccess, $"Expected success but got error: {result.Error?.Description}");
            Assert.Single(result.Value);
            Assert.Equal("Test Event", result.Value[0].Name);
        }


        [Fact]
        public async Task RegisterUserToEventUseCase_ShouldRegisterUserToEvent()
        {
            // Arrange
            var useCase = new RegisterUserToEventUseCase(_unitOfWorkMock.Object);
            var registerToEventDto = new RegisterToEventDto("test@example.com");
            var eventItem = new Event { Id = 1, Name = "Test Event", MaxMemberCount = 5, Users = new List<User>() };
            var user = new User { Id = 1, Email = "test@example.com" };

            _unitOfWorkMock.Setup(u => u.EventRepository.GetEventByIdAsync(It.IsAny<int>())).ReturnsAsync(eventItem);
            _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.EventRepository.UpdateEventAsync(eventItem)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

            // Act
            var result = await useCase.Execute(1, registerToEventDto);

            // Assert
            Assert.True(result.IsSuccess, $"Expected success but got error: {result.Error?.Description}");
            Assert.Contains(eventItem.Users, u => u.Email == "test@example.com");
        }

        [Fact]
        public async Task UpdateEventUseCase_ShouldUpdateEvent_WhenEventExists()
        {
            // Arrange
            var useCase = new UpdateEventUseCase(_unitOfWorkMock.Object, _mapperMock.Object);
            var eventDto = new EventDto { Id = 1, Name = "Updated Event" };
            var existingEvent = new Event { Id = 1, Name = "Old Event" };
            var updatedEventDto = new EventDto { Id = 1, Name = "Updated Event" };

            _unitOfWorkMock.Setup(u => u.EventRepository.GetEventByIdAsync(It.IsAny<int>())).ReturnsAsync(existingEvent);
            _mapperMock.Setup(m => m.Map(eventDto, existingEvent)).Callback(() => existingEvent.Name = eventDto.Name);
            _unitOfWorkMock.Setup(u => u.EventRepository.UpdateEventAsync(existingEvent)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<EventDto>(existingEvent)).Returns(updatedEventDto);

            // Act
            var result = await useCase.Execute(1, eventDto);

            // Assert
            Assert.True(result.IsSuccess, $"Expected success but got error: {result.Error?.Description}");
            Assert.Equal("Updated Event", result.Value.Name);
        }

        [Fact]
        public async Task CreateEventUseCase_ShouldCreateNewEvent()
        {
            // Arrange
            var useCase = new CreateEventUseCase(_unitOfWorkMock.Object, _mapperMock.Object);
            var eventDto = new EventDto
            {
                Id = 1,
                Name = "New Event",
                Description = "Test Description",
                Place = "Los Angeles",
                Type = "Workshop",
                MaxMemberCount = 10,
                Date = DateTime.UtcNow.AddDays(2),
                ImagePath = null
            };
            var newEvent = new Event 
            {
                Id = 1,
                Name = "New Event",
                Description = "Test Description",
                Place = "Los Angeles",
                Type = "Workshop",
                MaxMemberCount = 10,
                Date = DateTime.UtcNow.AddDays(2),
                ImagePath = null
            };
            var createdEventDto = new EventDto 
            {
                Id = 1,
                Name = "New Event",
                Description = "Test Description",
                Place = "Los Angeles",
                Type = "Workshop",
                MaxMemberCount = 10,
                Date = DateTime.UtcNow.AddDays(2),
                ImagePath = null
            };

            _mapperMock.Setup(m => m.Map<Event>(eventDto)).Returns(newEvent);
            _unitOfWorkMock.Setup(u => u.EventRepository.AddEventAsync(newEvent)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<EventDto>(newEvent)).Returns(createdEventDto);

            // Act
            var result = await useCase.Execute(eventDto);

            // Assert
            Assert.True(result.IsSuccess, $"Expected success but got error: {result.Error?.Description}");
            Assert.Equal("New Event", result.Value.Name);
        }

        [Fact]
        public async Task GetEventDetailsUseCase_ShouldReturnEventDetails_WhenEventExists()
        {
            // Arrange
            var useCase = new GetEventDetailsUseCase(_unitOfWorkMock.Object, _mapperMock.Object);
            var eventItem = new Event { Id = 1, Name = "Event Details Test" };
            var eventDto = new EventDto { Id = 1, Name = "Event Details Test" };

            _unitOfWorkMock.Setup(u => u.EventRepository.GetEventByIdAsync(It.IsAny<int>())).ReturnsAsync(eventItem);
            _mapperMock.Setup(m => m.Map<EventDto>(eventItem)).Returns(eventDto);

            // Act
            var result = await useCase.Execute(1);

            // Assert
            Assert.True(result.IsSuccess, $"Expected success but got error: {result.Error?.Description}");
            Assert.Equal("Event Details Test", result.Value.Name);
        }

        [Fact]
        public async Task DeleteEventUseCase_ShouldDeleteEvent_WhenEventExists()
        {
            // Arrange
            var useCase = new DeleteEventUseCase(_unitOfWorkMock.Object);
            var eventItem = new Event { Id = 1, Name = "Event to Delete" };

            _unitOfWorkMock.Setup(u => u.EventRepository.GetEventByIdAsync(It.IsAny<int>())).ReturnsAsync(eventItem);
            _unitOfWorkMock.Setup(u => u.EventRepository.DeleteEventAsync(eventItem)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

            // Act
            var result = await useCase.Execute(1);

            // Assert
            Assert.True(result.IsSuccess, $"Expected success but got error: {result.Error?.Description}");
            _unitOfWorkMock.Verify(u => u.EventRepository.DeleteEventAsync(eventItem), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
