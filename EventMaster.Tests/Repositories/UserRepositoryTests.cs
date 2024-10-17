using EventMaster.Server.Entities;
using EventMaster.Server.Repositories.Interfaces;
using EventMaster.Server.UnitOfWork;
using Moq;

namespace EventMaster.Tests.Repositories
{
    public class UserRepositoryTests
    {
        public Mock<IUnitOfWork> _unitOfWorkMock;

        public UserRepositoryTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
        }

        [Fact]
        public async Task AddUserRepositoryMethod_ShouldAddUserSuccessfully()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var newUser = new User { Id = 1, Email = "newuser@example.com" };
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepositoryMock.Object);
            userRepositoryMock.Setup(r => r.AddUserAsync(newUser)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveAsync());

            // Act
            await _unitOfWorkMock.Object.UserRepository.AddUserAsync(newUser);
            await _unitOfWorkMock.Object.SaveAsync();

            // Assert
            userRepositoryMock.Verify(r => r.AddUserAsync(It.Is<User>(u => u.Email == "newuser@example.com")));
            _unitOfWorkMock.Verify(u => u.SaveAsync());
        }

        [Fact]
        public async Task GetUserByIdRepositoryMethod_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var existingUser = new User { Id = 1, Email = "existinguser@example.com" };
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepositoryMock.Object);
            userRepositoryMock.Setup(r => r.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync(existingUser);

            // Act
            var result = await _unitOfWorkMock.Object.UserRepository.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("existinguser@example.com", result.Email);
        }
    }
}