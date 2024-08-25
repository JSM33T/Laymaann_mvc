use laymaann_db
GO
--=====================================================================
--tblMessages
DROP TABLE IF EXISTS tblMessages
DROP TABLE IF EXISTS tblUsers
DROP TABLE IF EXISTS tblRoles
DROP TABLE IF EXISTS tblAvatars
DROP TABLE IF EXISTS tblBlogCategories
DROP TABLE IF EXISTS tblBlogSeries
DROP TABLE IF EXISTS tblBlogPosts
DROP TABLE IF EXISTS tblBlogAuthors

--=====================================================================
--schema
CREATE TABLE [dbo].tblMessages (
    Id          INT PRIMARY KEY,
    UserId      INT,
	Name		NVARCHAR(512),
    [Message]   NVARCHAR(512)   not null,
	Mail		NVARCHAR(512),
	Topic		NVARCHAR(512),
    Origin      NVARCHAR(256) NOT NULL,
    UserAgent   NVARCHAR(512) NOT NULL,
    DateAdded   DATETIME DEFAULT (GETDATE())

	CONSTRAINT UQ_Mail_Message UNIQUE (Id, [Message])
);
GO
-- sp_ADD
CREATE OR ALTER PROCEDURE dbo.usp_AddMessage
    @Name NVARCHAR(128),
    @Mail NVARCHAR(128),
    @Message NVARCHAR(512),
    @Topic NVARCHAR(128),
    @Origin NVARCHAR(256),
    @UserAgent NVARCHAR(512),
    @DateAdded DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM tblMessages WHERE Mail = @Mail AND [Message] = @Message)
    BEGIN
        SELECT 0;
        RETURN;
    END

    DECLARE @NewId INT;
    SELECT @NewId = ISNULL(MAX(Id), 0) + 1 FROM tblMessages;

    INSERT INTO tblMessages (Id, Name, Mail, Message, Topic, Origin, UserAgent, DateAdded)
    VALUES (@NewId, @Name, @Mail, @Message, @Topic, @Origin, @UserAgent, @DateAdded);

    SELECT @NewId;
END
GO


--=====================================================================
--tblRoles
--=====================================================================
--schema
CREATE TABLE [dbo].tblRoles (
    Id				INT PRIMARY KEY,
    [Name]			NVARCHAR(64),
	Slug			NVARCHAR(64),
    [Description]   NVARCHAR(128),
    DateAdded		DATETIME DEFAULT (GETDATE())

	CONSTRAINT UQ_tblRoles_Role UNIQUE (Slug)
);
GO
-- seeding
INSERT INTO [dbo].tblRoles (Id, [Name], Slug, [Description], DateAdded)
VALUES 
    (1, 'User', 'user', 'Standard user role', GETDATE()),
    (2, 'Author', 'author', 'Content author role', GETDATE()),
    (3, 'Admin', 'admin', 'Administrator role', GETDATE());
GO


--=====================================================================
--tblUsers
--=====================================================================
CREATE TABLE [dbo].tblUsers (
    Id              INT PRIMARY KEY,
	GoogleId		NVARCHAR(64)		NOT NULL,
    Username        NVARCHAR(64),	
    Email           NVARCHAR(128)		NOT NULL,
    FirstName       NVARCHAR(64),
    LastName        NVARCHAR(64),
	Avatar			NVARCHAR(256),
    IsActive        BIT					NOT NULL	DEFAULT 1,
    RoleId          INT					NOT NULL	DEFAULT(1),
	[Key]			UNIQUEIDENTIFIER	NOT NULL	DEFAULT(NEWID()),

    DateAdded       DATETIME			NOT NULL	DEFAULT (GETDATE()),
	DateEdited      DATETIME			NOT NULL	DEFAULT (GETDATE()),

	CONSTRAINT	UQ_tblUsers_Username	UNIQUE		(Username),
	CONSTRAINT	UQ_tblUsers_Email		UNIQUE		(Email),
    CONSTRAINT	FK_tblUsers_Role		FOREIGN KEY (RoleId)	REFERENCES tblRoles(Id)
);
GO


-- usp_SignUpUser 'jsm33t','Jasmeet' ,'','jskainthofficial@gmail.com','asfhdkfjhasdkjlfhldjk'

GO
/****** Object:  StoredProcedure [dbo].[usp_SignUpUser]    Script Date: 30-06-2024 9.10.53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



--=====================================================================
--tblBlogCategories
--=====================================================================
CREATE TABLE [dbo].tblBlogCategories (
    Id				INT PRIMARY KEY,
    [Name]			NVARCHAR(64),
	Slug			NVARCHAR(64),
    [Description]   NVARCHAR(128),
    DateAdded		DATETIME DEFAULT (GETDATE())

	CONSTRAINT UQ_tblBlogCategories_Slug UNIQUE (Slug)
);
GO
--=====================================================================
-- Seed Data for tblBlogCategories
--=====================================================================
INSERT INTO dbo.tblBlogCategories (Id, [Name], Slug, [Description])
VALUES (0, 'Uncategorized', 'uncategorized', 'Default category');
GO

--=====================================================================
--tblBlogSeries
--=====================================================================
CREATE TABLE [dbo].tblBlogSeries (
    Id				INT PRIMARY KEY,
    [Name]			NVARCHAR(64),
	Slug			NVARCHAR(64),
    [Description]   NVARCHAR(128),
    DateAdded		DATETIME DEFAULT (GETDATE())

	CONSTRAINT UQ_tblBlogSeries_Slug UNIQUE (Slug)
);
GO

--=====================================================================
-- Seed Data for tblBlogSeries
--=====================================================================
INSERT INTO [dbo].[tblBlogSeries] (Id, [Name], Slug, [Description], DateAdded)
VALUES (0, 'Uncategorized', 'uncategorized', 'Default series', GETDATE());
GO

--=====================================================================
--tblBlogPosts
--=====================================================================
CREATE TABLE [dbo].tblBlogPosts (
    Id				INT PRIMARY KEY,
    Title			NVARCHAR(128),
	Slug			NVARCHAR(128),
    [Description]   NVARCHAR(128),
	Tags			NVARCHAR(128),
	Content			NVARCHAR(MAX),

	BlogSeriesId	INT NOT NULL DEFAULT(0),
	BlogCategory	INT NOT NULL DEFAULT(0),

	IsActive		BIT NOT NULL DEFAULT(0),
    DateAdded		DATETIME DEFAULT (GETDATE())

	CONSTRAINT UQ_tblBlogPosts_Slug UNIQUE (Slug)
);
GO

--=====================================================================
--tblBlogAuthors
--=====================================================================
CREATE TABLE [dbo].tblBlogAuthors (
    Id				INT PRIMARY KEY,
    BlogId			NVARCHAR(64),
	UserId			NVARCHAR(64),
    DateAdded		DATETIME DEFAULT (GETDATE())

	CONSTRAINT UQ_tblBlogAuthors_Authors UNIQUE (BlogId,UserId)
);
GO


CREATE OR ALTER PROCEDURE usp_AddOrUpdateUser
    @GoogleId NVARCHAR(64),
    @Username NVARCHAR(64),
    @Email NVARCHAR(128),
    @FirstName NVARCHAR(64),
    @LastName NVARCHAR(64),
    @Avatar NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    IF not EXISTS (SELECT 1 FROM tblUsers WHERE GoogleId = @GoogleId)
    --BEGIN
    --    UPDATE tblUsers
    --    SET
    --        Username = @Username,
    --        Email = @Email,
    --        FirstName = @FirstName,
    --        LastName = @LastName,
    --        Avatar = @Avatar,
    --        DateEdited = GETDATE()
    --    WHERE GoogleId = @GoogleId;
    --END
    --ELSE
    BEGIN
        DECLARE @NewId INT;
        SELECT @NewId = ISNULL(MAX(Id), 0) + 1 FROM tblUsers;

        INSERT INTO tblUsers (Id, GoogleId, Username, Email, FirstName, LastName, Avatar)
        VALUES (@NewId, @GoogleId, @Username, @Email, @FirstName, @LastName, @Avatar);
    END

	SELECT * FROM tblUsers WHERE GoogleId = @GoogleId;
END
GO



CREATE OR ALTER PROCEDURE usp_GetUserByGoogleId
    @GoogleId NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM tblUsers WHERE GoogleId = @GoogleId)
    BEGIN
	SELECT * FROM tblUsers WHERE GoogleId = @GoogleId AND IsActive = 1
	END
	;
END
GO


CREATE or alter PROCEDURE usp_GetAllUsers
AS
BEGIN
    SELECT 
        Id, 
        GoogleId, 
        Username, 
        Email, 
        FirstName, 
        LastName, 
        Avatar AS ProfilePicture, 
        RoleId
    FROM 
        tblUsers;
END
GO

CREATE OR ALTER PROCEDURE usp_GetBlogPostByYearAndSlug
    @Year INT,
    @Slug NVARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM tblBlogPosts
    WHERE YEAR(DateAdded) = @Year
      AND Slug = @Slug
      AND IsActive = 1;
END
GO

CREATE or alter PROCEDURE usp_AddFeedbackMessage
    @UserId NVARCHAR(128),
    @Origin NVARCHAR(256),
    @Message NVARCHAR(MAX),
    @UserAgent NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NextID INT;
    SELECT @NextID = ISNULL(MAX(ID), 0) + 1 FROM tblMessages;

    INSERT INTO tblMessages (ID, UserId, Origin, Message, UserAgent, DateAdded)
    VALUES (@NextID, @UserId, @Origin, @Message, @UserAgent, GETDATE());
END
GO

CREATE or alter PROCEDURE usp_GetUserCount
 
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) as UserCount
    FROM tblUsers 
END
GO