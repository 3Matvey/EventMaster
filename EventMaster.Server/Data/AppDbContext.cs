using EventMaster.Server.Services.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventMaster.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<User> Users { get; set; }
    }
}