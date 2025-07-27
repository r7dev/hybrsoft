CREATE TABLE [Learn].[StudentRelative]
(
	[StudentRelativeID] BIGINT NOT NULL PRIMARY KEY,
	[StudentID] BIGINT NOT NULL,
	[RelativeID] BIGINT NOT NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(350) NULL,
	CONSTRAINT FK_StudentRelative_StudentID FOREIGN KEY ([StudentID]) REFERENCES [Learn].[Student] ([StudentID]) ON DELETE CASCADE,
	CONSTRAINT FK_StudentRelative_RelativeID FOREIGN KEY ([RelativeID]) REFERENCES [Learn].[Relative] ([RelativeID]),
	CONSTRAINT UQ_StudentRelative_StudentID_RelativeID UNIQUE ([StudentID], [RelativeID])
)
GO
CREATE INDEX IX_StudentRelative_StudentID_RelativeID_SearchTerms
ON [Learn].[StudentRelative] ([StudentID], [RelativeID], [SearchTerms]);
