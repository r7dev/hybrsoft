CREATE TABLE [Learn].[StudentBelonging]
(
	[StudentBelongingID] BIGINT NOT NULL PRIMARY KEY,
	[StudentID] BIGINT NOT NULL,
	[DisplayName] NVARCHAR(50) NOT NULL,
	[Description] NVARCHAR(2000) NOT NULL,
	[Picture] VARBINARY(MAX) NULL,
	[Thumbnail] VARBINARY(MAX) NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(2000) NULL,
	[SearchTermsStudent] NVARCHAR(2200) NULL,
	CONSTRAINT FK_StudentBelonging_StudentID FOREIGN KEY ([StudentID]) REFERENCES [Learn].[Student] ([StudentID]) ON DELETE CASCADE
)
GO
CREATE INDEX IX_StudentBelonging_DisplayName_SearchTerms
ON [Learn].[StudentBelonging] ([DisplayName], [SearchTerms])
INCLUDE ([StudentBelongingID], [StudentID], [Thumbnail]);
