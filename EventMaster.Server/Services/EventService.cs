using EventMaster.Server.Services.Entities;

namespace EventMaster.Server.Services
{
    public class EventService
    {
        private readonly List<Event> _events =
        [
            new Event
            {
                Name = "First Event",
                Description = "Description of the first event",
                Date = DateTime.Now.AddDays(1),
                Place = "Conference Hall A",
                Type = "Conference",
                MaxMemberCount = 100,
                Users = new List<User>()
            },
            new Event
            {
                Name = "Second Event",
                Description = "Description of the second event",
                Date = DateTime.Now.AddDays(2),
                Place = "Auditorium B",
                Type = "Workshop",
                MaxMemberCount = 50,
                Users = new List<User>()
            },
            new Event
            {
                Name = "Third",
                Description = "A special networking event for professionals",
                Date = DateTime.Now.AddDays(5),
                Place = "City Hall",
                Type = "Networking",
                MaxMemberCount = 200,
                Users = new List<User>()
            },
            new Event
            {
                Name = "Fourth Event",
                Description = "A hands-on coding workshop for beginners",
                Date = DateTime.Now.AddDays(7),
                Place = "Tech Park",
                Type = "Coding Workshop",
                MaxMemberCount = 30,
                Users = new List<User>()
            }
        ];


        // Получение всех событий
        public List<Event> GetAllEvents()
        {
            return _events;
        }

        // Поиск события по имени
        public Event GetEventByName(string name)
        {
            return _events.FirstOrDefault(e => e.Name == name);
        }

        // Добавление нового события
        public void AddEvent(Event newEvent)
        {
            _events.Add(newEvent);
        }

        // Обновление события
        public bool UpdateEvent(string name, Event updatedEvent)
        {
            var index = _events.FindIndex(e => e.Name == name);
            if (index == -1)
            {
                return false;
            }

            _events[index] = updatedEvent;
            return true;
        }

        // Удаление события
        public bool DeleteEvent(string name)
        {
            var eventItem = _events.FirstOrDefault(e => e.Name == name);
            if (eventItem == null)
            {
                return false;
            }

            _events.Remove(eventItem);
            return true;
        }
    }
}
