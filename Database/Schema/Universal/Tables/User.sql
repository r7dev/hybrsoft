CREATE TABLE [Universal].[User]
(
	[UserId] BIGINT NOT NULL PRIMARY KEY,
	[FirstName] NVARCHAR(50) NOT NULL,
	[LastName] NVARCHAR(50) NOT NULL,
	[Email] NVARCHAR(150) NOT NULL UNIQUE,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(300) NULL
)
GO
CREATE INDEX IX_User_FirstName ON [Universal].[User] (FirstName);
GO
CREATE INDEX IX_User_SearchTerms ON [Universal].[User] (SearchTerms);
