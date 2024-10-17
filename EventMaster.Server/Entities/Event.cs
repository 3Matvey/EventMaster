namespace EventMaster.Server.Entities
{
    public class Event
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Place { get; set; }
        public string Type { get; set; }
        public int MaxMemberCount { get; set; }
        public List<User> Users { get; set; } = [];
        public string? ImagePath { get; set; }
    }
}
