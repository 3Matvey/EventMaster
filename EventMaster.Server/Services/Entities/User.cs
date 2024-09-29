using System.ComponentModel.DataAnnotations;

namespace EventMaster.Server.Services.Entities
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime DateOf { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public List<Event> RegisteredEvents { get; set; } = [];
    }
}
