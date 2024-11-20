using System.ComponentModel.DataAnnotations;

namespace EventMaster.Domain.Entities
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Place is required.")]
        [StringLength(200, ErrorMessage = "Place can't be longer than 200 characters.")]
        public string Place { get; set; } = string.Empty;

        [Required(ErrorMessage = "Type is required.")]
        [StringLength(50, ErrorMessage = "Type can't be longer than 50 characters.")]
        public string Type { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "MaxMemberCount must be at least 1.")]
        public int MaxMemberCount { get; set; }

        public ICollection<User> Users { get; set; } = [];

        [Url(ErrorMessage = "ImagePath must be a valid URL.")]
        public string? ImagePath { get; set; }
    }
}
