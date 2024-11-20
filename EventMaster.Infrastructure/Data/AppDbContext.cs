using EventMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventMaster.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().Ignore(u => u.RefreshToken);
            modelBuilder.Entity<User>().Ignore(u => u.RefreshTokenExpiryDate);
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    // Добавление дефолтных пользователей
        //    modelBuilder.Entity<User>().HasData(
        //        new User
        //        {
        //            Id = 1,
        //            FirstName = "John",
        //            LastName = "Doe",
        //            BirthDate = new DateTime(1990, 5, 15),
        //            DateOf = DateTime.Now,
        //            Email = "john.doe@example.com",
        //            Role = "User",
        //            Password = "123" 
        //        },
        //        new User
        //        {
        //            Id = 2,
        //            FirstName = "Jane",
        //            LastName = "Smith",
        //            BirthDate = new DateTime(1985, 8, 23),
        //            DateOf = DateTime.Now,
        //            Email = "jane.smith@example.com",
        //            Role = "Admin",
        //            Password = "456"
        //        },
        //        new User
        //        {
        //            Id = 3,
        //            FirstName = "Alex",
        //            LastName = "Johnson",
        //            BirthDate = new DateTime(1993, 10, 2),
        //            DateOf = DateTime.Now,
        //            Email = "alex.johnson@example.com",
        //            Role = "User",
        //            Password = "789"
        //        },
        //        new User
        //        {
        //            Id = 4,
        //            FirstName = "Maria",
        //            LastName = "Kovalsky",
        //            BirthDate = new DateTime(1992, 11, 12),
        //            DateOf = DateTime.Now,
        //            Email = "maria.kovalsky@example.com",
        //            Role = "User",
        //            Password = "321"
        //        }
        //    );

        //    // Добавление дефолтных событий
        //    modelBuilder.Entity<Event>().HasData(
        //        new Event
        //        {
        //            Id = 1,
        //            Name = "First Event",
        //            Description = "Description of the first event",
        //            Date = new DateTime(2024, 10, 7),
        //            Place = "Conference Hall A",
        //            Type = "Conference",
        //            MaxMemberCount = 100,
        //            ImagePath = null
        //        },
        //        new Event
        //        {
        //            Id = 2,
        //            Name = "Second Event",
        //            Description = "Description of the second event",
        //            Date = new DateTime(2024, 10, 10),
        //            Place = "Auditorium B",
        //            Type = "Workshop",
        //            MaxMemberCount = 50,
        //            ImagePath = null
        //        },
        //        new Event
        //        {
        //            Id = 3,
        //            Name = "Networking Event",
        //            Description = "A special networking event for professionals",
        //            Date = new DateTime(2024, 10, 15),
        //            Place = "City Hall",
        //            Type = "Networking",
        //            MaxMemberCount = 200,
        //            ImagePath = null
        //        },
        //        new Event
        //        {
        //            Id = 4,
        //            Name = "Coding Workshop",
        //            Description = "A hands-on coding workshop for beginners",
        //            Date = new DateTime(2024, 10, 20),
        //            Place = "Tech Park",
        //            Type = "Coding Workshop",
        //            MaxMemberCount = 30,
        //            ImagePath = null
        //        }
        //    );

        //    base.OnModelCreating(modelBuilder);
        //}
    }
}
