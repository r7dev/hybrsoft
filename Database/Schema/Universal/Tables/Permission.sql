CREATE TABLE [Universal].[Permission]
(
	[PermissionId] BIGINT NOT NULL PRIMARY KEY,
	[Name] NVARCHAR(50) NOT NULL UNIQUE,
	[DisplayName] NVARCHAR(50) NOT NULL,
	[Description] NVARCHAR(150) NULL,
	[IsEnabled] BIT NOT NULL DEFAULT 1,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(300) NULL
)
GO
CREATE INDEX IX_Permission_Name ON [Universal].[Permission] (Name);
GO
CREATE INDEX IX_Permission_SearchTerms ON [Universal].[Permission] (SearchTerms);
