using System.ComponentModel.DataAnnotations;

namespace EventMaster.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "FirstName is required.")]
        [StringLength(50, ErrorMessage = "FirstName can't be longer than 50 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "LastName is required.")]
        [StringLength(50, ErrorMessage = "LastName can't be longer than 50 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "BirthDate is required.")]
        public DateTime BirthDate { get; set; }

        public DateTime DateOf { get; set; } 

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required.")]
        [StringLength(20, ErrorMessage = "Role can't be longer than 20 characters.")]
        public string Role { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "Password can't be longer than 100 characters.")]
        public string Password { get; set; } = string.Empty;

        public ICollection<Event> RegisteredEvents { get; set; } = [];
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryDate { get; set; }
    }
}
