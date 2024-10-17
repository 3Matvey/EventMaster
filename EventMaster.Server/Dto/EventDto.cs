namespace EventMaster.Server.Dto
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Place { get; set; }
        public string Type { get; set; }
        public int MaxMemberCount { get; set; }
        public DateTime Date { get; set; }
        public string? ImagePath { get; set; }
    }

}
