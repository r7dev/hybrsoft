CREATE TABLE [Universal].[Role]
(
	[RoleId] BIGINT NOT NULL PRIMARY KEY,
	[Name] NVARCHAR(50) NOT NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(100) NULL
)
GO
CREATE INDEX IX_Role_Name_SearchTerms ON [Universal].[Role] ([Name], [SearchTerms]);
