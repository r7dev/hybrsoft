CREATE TABLE [Universal].[Permission]
(
	[PermissionID] BIGINT NOT NULL PRIMARY KEY,
	[Name] NVARCHAR(50) NOT NULL,
	[DisplayName] NVARCHAR(50) NOT NULL,
	[Description] NVARCHAR(150) NULL,
	[IsEnabled] BIT NOT NULL DEFAULT 1,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(300) NULL,
	CONSTRAINT UQ_Permission_Name UNIQUE ([Name])
)
GO
CREATE INDEX IX_Permission_Name_SearchTerms
ON [Universal].[Permission] ([Name], [SearchTerms])
INCLUDE ([DisplayName], [Description]);
GO
CREATE INDEX IX_Permission_DisplayName
ON [Universal].[Permission] ([DisplayName]);
