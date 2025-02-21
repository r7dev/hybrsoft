CREATE TABLE [Learn].[Student]
(
	[StudentId] BIGINT NOT NULL PRIMARY KEY,
	[FirstName] NVARCHAR(50) NOT NULL,
	[MiddleName] NVARCHAR(50) NULL,
	[LastName] NVARCHAR(50) NOT NULL,
	[Email] NVARCHAR(150) NOT NULL UNIQUE,
	[Password] NVARCHAR(100) NOT NULL,
	[PasswordLength] INT NOT NULL DEFAULT 0,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(300) NULL
)
GO
CREATE INDEX IX_Student_FirstName ON [Learn].[Student] (FirstName);
GO
CREATE INDEX IX_Student_SearchTerms ON [Learn].[Student] (SearchTerms);
