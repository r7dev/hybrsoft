CREATE TABLE [Learn].[Dismissal]
(
	[DismissalId] BIGINT NOT NULL PRIMARY KEY,
	[ClassroomId] BIGINT NOT NULL,
	[StudentId] BIGINT NOT NULL,
	[RelativeId] BIGINT NOT NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[DismissedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(200) NULL,
	CONSTRAINT FK_Dismissal_ClassroomId FOREIGN KEY ([ClassroomId]) REFERENCES [Learn].[Classroom] ([ClassroomId]),
	CONSTRAINT FK_Dismissal_StudentId FOREIGN KEY ([StudentId]) REFERENCES [Learn].[Student] ([StudentId]),
	CONSTRAINT FK_Dismissal_RelativeId FOREIGN KEY ([RelativeId]) REFERENCES [Learn].[Relative] ([RelativeId]),
)
GO
CREATE INDEX IX_Dismissal_CreatedOn_SearchTerms
ON [Learn].[Dismissal] ([CreatedOn] DESC, [SearchTerms])
INCLUDE ([DismissalId], [ClassroomId], [StudentId], [RelativeId], [DismissedOn])
WHERE [DismissedOn] IS NULL;
