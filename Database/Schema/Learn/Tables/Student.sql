CREATE TABLE [Learn].[Student]
(
	[StudentId] BIGINT NOT NULL PRIMARY KEY,
	[FirstName] NVARCHAR(50) NOT NULL,
	[MiddleName] NVARCHAR(50) NULL,
	[LastName] NVARCHAR(50) NOT NULL,
	[Email] NVARCHAR(150) NOT NULL,
	[Picture] VARBINARY(MAX) NULL,
	[Thumbnail] VARBINARY(MAX) NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(350) NULL,
	CONSTRAINT UQ_Student_Email UNIQUE ([Email]),
)
GO
CREATE INDEX IX_Student_FirstName ON [Learn].[Student] (FirstName);
GO
CREATE INDEX IX_Student_SearchTerms ON [Learn].[Student] (SearchTerms);
