CREATE TABLE [Learn].[StudentBelonging]
(
	[StudentBelongingID] BIGINT NOT NULL PRIMARY KEY,
	[StudentID] BIGINT NOT NULL,
	[DisplayName] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(4000) NOT NULL,
	[Picture] VARBINARY(MAX) NULL,
	[Thumbnail] VARBINARY(MAX) NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(4000) NULL,
	CONSTRAINT FK_StudentBelonging_StudentID FOREIGN KEY ([StudentID]) REFERENCES [Learn].[Student] ([StudentID]) ON DELETE CASCADE
)
GO
CREATE INDEX IX_StudentBelonging_StudentID_SearchTerms
ON [Learn].[StudentBelonging] ([StudentID], [SearchTerms]);
