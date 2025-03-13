-- Удаляем таблицы, если они уже существуют
IF OBJECT_ID('dbo.Events', 'U') IS NOT NULL 
    DROP TABLE dbo.Events;

IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL 
    DROP TABLE dbo.Users;

-- Создаем таблицу Users
CREATE TABLE dbo.Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    BirthDate DATE NOT NULL,
    DateOf DATETIME NOT NULL DEFAULT GETDATE(),
    Email NVARCHAR(255) NOT NULL,
    Role NVARCHAR(50) NOT NULL,
    Password NVARCHAR(100) NOT NULL
);

-- Создаем таблицу Events
CREATE TABLE dbo.Events (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Date DATE NOT NULL,
    Place NVARCHAR(200) NOT NULL,
    Type NVARCHAR(100) NOT NULL,
    MaxMemberCount INT NOT NULL,
    ImagePath NVARCHAR(255) NULL
);

-- Вставляем данные в таблицу Users
INSERT INTO dbo.Users (FirstName, LastName, BirthDate, DateOf, Email, Role, Password)
VALUES
('John', 'Doe', '1990-05-15', GETDATE(), 'john.doe@example.com', 'User', '123'),
('Jane', 'Smith', '1985-08-23', GETDATE(), 'jane.smith@example.com', 'Admin', '456'),
('Alex', 'Johnson', '1993-10-02', GETDATE(), 'alex.johnson@example.com', 'User', '789'),
('Maria', 'Kovalsky', '1992-11-12', GETDATE(), 'maria.kovalsky@example.com', 'User', '321');

-- Вставляем данные в таблицу Events
INSERT INTO dbo.Events (Name, Description, Date, Place, Type, MaxMemberCount, ImagePath)
VALUES
('First Event', 'Description of the first event', '2024-10-07', 'Conference Hall A', 'Conference', 100, NULL),
('Second Event', 'Description of the second event', '2024-10-10', 'Auditorium B', 'Workshop', 50, NULL),
('Networking Event', 'A special networking event for professionals', '2024-10-15', 'City Hall', 'Networking', 200, NULL),
('Coding Workshop', 'A hands-on coding workshop for beginners', '2024-10-20', 'Tech Park', 'Coding Workshop', 30, NULL);
