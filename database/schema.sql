-- Run only against a newly created, empty PortfolioWeek5 database.
-- This script never creates, drops, or resets an existing database.
IF DB_NAME() <> N'PortfolioWeek5'
    THROW 50000, 'Select the new PortfolioWeek5 database first.', 1;
IF OBJECT_ID(N'dbo.Students', N'U') IS NOT NULL OR OBJECT_ID(N'dbo.Programs', N'U') IS NOT NULL
    THROW 50001, 'Expected an empty database; no changes made.', 1;
SET XACT_ABORT ON;
BEGIN TRANSACTION;
CREATE TABLE dbo.Programs (
    ProgramId int IDENTITY PRIMARY KEY,
    ProgramCode nvarchar(20) NOT NULL UNIQUE,
    ProgramName nvarchar(100) NOT NULL,
    IsActive bit NOT NULL DEFAULT 1
);
CREATE TABLE dbo.Students (
    StudentId int IDENTITY PRIMARY KEY,
    NIM nvarchar(50) NOT NULL UNIQUE,
    Name nvarchar(200) NOT NULL,
    ProgramId int NOT NULL REFERENCES dbo.Programs(ProgramId),
    Gender nvarchar(30) NULL,
    Email nvarchar(254) NULL,
    BirthDate date NULL,
    Address nvarchar(500) NULL,
    PhoneNumber nvarchar(50) NULL,
    CreatedAt datetime2 NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt datetime2 NULL
);
INSERT INTO dbo.Programs (ProgramCode, ProgramName)
VALUES (N'CS', N'Teknik Informatika'), (N'IS', N'Sistem Informasi');
COMMIT TRANSACTION;
