Для полноценной проверки работоспособности системы нужно добавить пользователей и события, например выполнив
```
INSERT INTO Users (FirstName, LastName, BirthDate, DateOf, Email, Role, Password)
VALUES
('John', 'Doe', '1990-05-15', GETDATE(), 'john.doe@example.com', 'User', '123'),
('Jane', 'Smith', '1985-08-23', GETDATE(), 'jane.smith@example.com', 'Admin', '456'),
('Alex', 'Johnson', '1993-10-02', GETDATE(), 'alex.johnson@example.com', 'User', '789'),
('Maria', 'Kovalsky', '1992-11-12', GETDATE(), 'maria.kovalsky@example.com', 'User', '321');

INSERT INTO Events (Name, Description, Date, Place, Type, MaxMemberCount, ImagePath)
VALUES
('First Event', 'Description of the first event', '2024-10-07', 'Conference Hall A', 'Conference', 100, NULL),
('Second Event', 'Description of the second event', '2024-10-10', 'Auditorium B', 'Workshop', 50, NULL),
('Networking Event', 'A special networking event for professionals', '2024-10-15', 'City Hall', 'Networking', 200, NULL),
('Coding Workshop', 'A hands-on coding workshop for beginners', '2024-10-20', 'Tech Park', 'Coding Workshop', 30, NULL);
```
