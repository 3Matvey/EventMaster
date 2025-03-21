-- Create EventMasterDB database if it does not exist
IF NOT EXISTS (
    SELECT name 
    FROM sys.databases 
    WHERE name = N'EventMasterDB'
)
BEGIN
    CREATE DATABASE [EventMasterDB];
END;
GO

-- Switch to the new database
USE [EventMasterDB];
GO

-- Create Users table (if not exists)
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        UserID   INT           IDENTITY(1,1) PRIMARY KEY,  -- auto-increment ID&#8203;:contentReference[oaicite:14]{index=14}
        UserName NVARCHAR(100) NOT NULL,
        Email    NVARCHAR(100) NULL
    );
END;
GO

-- Create Events table (if not exists), with foreign key linking to Users
IF OBJECT_ID(N'dbo.Events', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Events (
        EventID     INT           IDENTITY(1,1) PRIMARY KEY,
        EventName   NVARCHAR(100) NOT NULL,
        EventDate   DATETIME      NOT NULL,
        OrganizerID INT           NOT NULL,
        FOREIGN KEY (OrganizerID) REFERENCES dbo.Users(UserID)
    );
END;
GO

-- Insert initial Users (only if table is empty)
IF NOT EXISTS (SELECT 1 FROM dbo.Users)
BEGIN
    INSERT INTO dbo.Users (UserName, Email)
    VALUES 
        (N'Alice', N'alice@example.com'),
        (N'Bob',   N'bob@example.com');
END;
GO

-- Insert initial Events (only if table is empty)
IF NOT EXISTS (SELECT 1 FROM dbo.Events)
BEGIN
    INSERT INTO dbo.Events (EventName, EventDate, OrganizerID)
    VALUES
        (N'Conference 2025', '2025-03-20', 1),
        (N'Workshop 2025',   '2025-04-15', 2);
END;
GO
