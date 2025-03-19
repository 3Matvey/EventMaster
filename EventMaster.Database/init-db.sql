-- EventMaster.Database/init-db.sql

CREATE DATABASE EventMasterDB;
GO

USE EventMasterDB;
GO

CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY,
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL
);
GO

CREATE TABLE Events (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Date DATETIME NOT NULL,
    Location NVARCHAR(255)
);
GO

-- Добавьте начальные данные, если необходимо
INSERT INTO Users (Username, PasswordHash, Email) VALUES ('admin', 'hashedpassword', 'admin@example.com');
INSERT INTO Events (Name, Description, Date, Location) VALUES ('Sample Event', 'This is a sample event.', GETDATE(), 'Sample Location');
GO

-- Содержимое StarterKit.sql
INSERT INTO Users (FirstName, LastName, BirthDate, DateOf, Email, Role, Password)
VALUES
('John', 'Doe', '1990-05-15', GETDATE(), 'john.doe@example.com', 'User', '123'),
('Jane', 'Smith', '1985-08-23', GETDATE(), 'jane.smith@example.com', 'Admin', '456'),
('Alex', 'Johnson', '1993-10-02', GETDATE(), 'alex.johnson@example.com', 'User', '789'),
('Maria', 'Kovalsky', '1992-11-12', GETDATE(), 'maria.kovalsky@example.com', 'User', '321');
GO

INSERT INTO Events (Name, Description, Date, Place, Type, MaxMemberCount, ImagePath)
VALUES
('First Event', 'Description of the first event', '2024-10-07', 'Conference Hall A', 'Conference', 100, NULL),
('Second Event', 'Description of the second event', '2024-10-10', 'Auditorium B', 'Workshop', 50, NULL),
('Networking Event', 'A special networking event for professionals', '2024-10-15', 'City Hall', 'Networking', 200, NULL),
('Coding Workshop', 'A hands-on coding workshop for beginners', '2024-10-20', 'Tech Park', 'Coding Workshop', 30, NULL);
GO
