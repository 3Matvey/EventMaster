using EventMaster.Server.Services.Entities;

namespace EventMaster.Server.Services
{
    public class UserService
    {
        private readonly List<User> _users =
        [
            new User
            {
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(1990, 5, 15),
                DateOf = DateTime.Now,
                Email = "john.doe@example.com",
                Password = "123",
                Role = "User",
                RegisteredEvents = []
            },
            new User
            {
                FirstName = "Jane",
                LastName = "Smith",
                BirthDate = new DateTime(1985, 8, 23),
                DateOf = DateTime.Now,
                Email = "jane.smith@example.com",
                Password = "456",
                Role = "Admin",
                RegisteredEvents = []
            },
            new User
            {
                FirstName = "Alex",
                LastName = "Johnson",
                BirthDate = new DateTime(1993, 10, 2),
                DateOf = DateTime.Now,
                Email = "alex.johnson@example.com",
                Password = "789",
                Role = "User",
                RegisteredEvents = []
            },
            new User
            {
                FirstName = "Sanya",
                LastName = "Volchok",
                BirthDate = new DateTime(2005, 5, 18),
                DateOf = DateTime.Now,
                Email = "f104flyingcoffin@mail.com",
                Password = "password88",
                Role = "Admin",
                RegisteredEvents = []
            },
            new User
            {
                FirstName = "Maria",
                LastName = "Kovalsky",
                BirthDate = new DateTime(1992, 11, 12),
                DateOf = DateTime.Now,
                Email = "maria.kovalsky@example.com",
                Password = "321",
                Role = "User",
                RegisteredEvents = []
            }
        ];

        // Получение всех пользователей
        public List<User> GetAllUsers()
        {
            return _users;
        }

        // Поиск пользователя по Email
        public User? GetUserByEmail(string email)
        {
            return _users?.FirstOrDefault(u => u.Email == email);
        }

        // Добавление нового пользователя
        public void AddUser(User user)
        {
            _users.Add(user);
        }

        // Проверка, существует ли пользователь с таким Email
        public bool UserExists(string email)
        {
            return _users.Any(u => u.Email == email);
        }
    }
}
