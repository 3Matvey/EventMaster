namespace EventMaster.Application.DTOs
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Place { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int MaxMemberCount { get; set; }
        public DateTime Date { get; set; }
        public string? ImagePath { get; set; }
    }

}
