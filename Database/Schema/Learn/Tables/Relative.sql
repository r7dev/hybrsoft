CREATE TABLE [Learn].[Relative]
(
	[RelativeID] BIGINT NOT NULL PRIMARY KEY,
	[FirstName] NVARCHAR(50) NOT NULL,
	[MiddleName] NVARCHAR(50) NULL,
	[LastName] NVARCHAR(50) NOT NULL,
	[RelativeTypeID] SMALLINT NOT NULL,
	[DocumentNumber] NVARCHAR(20) NULL,
	[Phone] NVARCHAR(20) NULL,
	[Email] NVARCHAR(70) NULL,
	[Picture] VARBINARY(MAX) NULL,
	[Thumbnail] VARBINARY(MAX) NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(350) NULL,
	CONSTRAINT FK_Relative_RelativeTypeID FOREIGN KEY ([RelativeTypeID]) REFERENCES [Learn].[RelativeType] ([RelativeTypeID]),
)
GO
CREATE INDEX IX_Relative_FirstName_SearchTerms
ON [Learn].[Relative] ([FirstName], [SearchTerms])
INCLUDE ([LastName], [Thumbnail], [LastModifiedOn]);
