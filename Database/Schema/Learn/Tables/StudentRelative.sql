CREATE TABLE [Learn].[StudentRelative]
(
	[StudentRelativeId] BIGINT NOT NULL PRIMARY KEY,
	[StudentId] BIGINT NOT NULL,
	[RelativeId] BIGINT NOT NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(350) NULL,
	CONSTRAINT FK_StudentRelative_StudentId FOREIGN KEY ([StudentId]) REFERENCES [Learn].[Student] ([StudentId]) ON DELETE CASCADE,
	CONSTRAINT FK_StudentRelative_RelativeId FOREIGN KEY ([RelativeId]) REFERENCES [Learn].[Relative] ([RelativeId]),
	CONSTRAINT UQ_StudentRelative_StudentId_RelativeId UNIQUE ([StudentId], [RelativeId])
)
GO
CREATE INDEX IX_StudentRelative_StudentId_RelativeId_SearchTerms
ON [Learn].[StudentRelative] ([StudentId], [RelativeId], [SearchTerms]);
