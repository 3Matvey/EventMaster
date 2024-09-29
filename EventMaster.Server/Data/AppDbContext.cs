using EventMaster.Server.Services.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventMaster.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet для сущностей
        public DbSet<Event> Events { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
