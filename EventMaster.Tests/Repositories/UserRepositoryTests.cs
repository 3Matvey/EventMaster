using EventMaster.Server.Data;
using EventMaster.Server.Repositories.Implementation;
using EventMaster.Server.Services.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EventMaster.Tests.Repositories
{
    public class UserRepositoryTests
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
        public async Task AddUserAsync_ShouldAddUserToDatabase()
        {
            // Arrange
            var context = await GetInMemoryDbContextAsync();
            var userRepository = new UserRepository(context);
            var user = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "password123",
                Role = "User"
            };

            // Act
            await userRepository.AddUserAsync(user);

            // Assert
            var addedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "john.doe@example.com");
            Assert.NotNull(addedUser);
            Assert.True(addedUser.Id > 0);
            Assert.Equal(user.FirstName, addedUser.FirstName);
            Assert.Equal(user.LastName, addedUser.LastName);
            Assert.Equal(user.Email, addedUser.Email);
            Assert.Equal(user.Role, addedUser.Role);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnCorrectUser()
        {
            // Arrange
            var context = await GetInMemoryDbContextAsync();
            var userRepository = new UserRepository(context);
            var user = new User
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com",
                Password = "password123",
                Role = "User"
            };
            await userRepository.AddUserAsync(user);

            // Act
            var retrievedUser = await userRepository.GetUserByIdAsync(user.Id);

            // Assert
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.FirstName, retrievedUser.FirstName);
            Assert.Equal(user.LastName, retrievedUser.LastName);
            Assert.Equal(user.Email, retrievedUser.Email);
            Assert.Equal(user.Role, retrievedUser.Role);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUserDetails()
        {
            // Arrange
            var context = await GetInMemoryDbContextAsync();
            var userRepository = new UserRepository(context);
            var user = new User
            {
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice.smith@example.com",
                Password = "password123",
                Role = "User"
            };
            await userRepository.AddUserAsync(user);

            // Act
            user.LastName = "Johnson";
            user.Role = "Admin";
            var updateResult = await userRepository.UpdateUserAsync(user);

            // Assert
            Assert.True(updateResult);
            var updatedUser = await userRepository.GetUserByIdAsync(user.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal("Johnson", updatedUser.LastName);
            Assert.Equal("Admin", updatedUser.Role);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldRemoveUserFromDatabase()
        {
            // Arrange
            var context = await GetInMemoryDbContextAsync();
            var userRepository = new UserRepository(context);
            var user = new User
            {
                FirstName = "Bob",
                LastName = "Brown",
                Email = "bob.brown@example.com",
                Password = "password123",
                Role = "User"
            };
            await userRepository.AddUserAsync(user);

            // Act
            var deleteResult = await userRepository.DeleteUserAsync(user.Id);

            // Assert
            Assert.True(deleteResult);
            var deletedUser = await userRepository.GetUserByIdAsync(user.Id);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task UserExistsAsync_ShouldReturnTrueIfUserExists()
        {
            // Arrange
            var context = await GetInMemoryDbContextAsync();
            var userRepository = new UserRepository(context);
            var user = new User
            {
                FirstName = "Charlie",
                LastName = "White",
                Email = "charlie.white@example.com",
                Password = "password123",
                Role = "User"
            };
            await userRepository.AddUserAsync(user);

            // Act
            var exists = await userRepository.UserExistsAsync(user.Email);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task UserExistsAsync_ShouldReturnFalseIfUserDoesNotExist()
        {
            // Arrange
            var context = await GetInMemoryDbContextAsync();
            var userRepository = new UserRepository(context);

            // Act
            var exists = await userRepository.UserExistsAsync("nonexistent@example.com");

            // Assert
            Assert.False(exists);
        }
    }
}