CREATE TABLE [Learn].[Relative]
(
	[RelativeId] BIGINT NOT NULL PRIMARY KEY,
	[FirstName] NVARCHAR(50) NOT NULL,
	[MiddleName] NVARCHAR(50) NULL,
	[LastName] NVARCHAR(50) NOT NULL,
	[RelativeTypeId] SMALLINT NOT NULL,
	[DocumentNumber] NVARCHAR(20) NULL,
	[Phone] NVARCHAR(20) NULL,
	[Email] NVARCHAR(70) NULL,
	[Picture] VARBINARY(MAX) NULL,
	[Thumbnail] VARBINARY(MAX) NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(350) NULL,
	CONSTRAINT FK_Relative_RelativeTypeId FOREIGN KEY ([RelativeTypeId]) REFERENCES [Learn].[RelativeType] ([RelativeTypeId]),
)
GO
CREATE INDEX IX_Relative_FirstName ON [Learn].[Relative] ([FirstName]);
GO
CREATE INDEX IX_Relative_SearchTerms ON [Learn].[Relative] ([SearchTerms]);
