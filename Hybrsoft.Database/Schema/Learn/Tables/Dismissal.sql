CREATE TABLE [Learn].[Dismissal]
(
	[DismissalID] BIGINT NOT NULL PRIMARY KEY,
	[ClassroomID] BIGINT NOT NULL,
	[StudentID] BIGINT NOT NULL,
	[RelativeID] BIGINT NOT NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[DismissedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(200) NULL,
	CONSTRAINT FK_Dismissal_ClassroomID FOREIGN KEY ([ClassroomID]) REFERENCES [Learn].[Classroom] ([ClassroomID]),
	CONSTRAINT FK_Dismissal_StudentID FOREIGN KEY ([StudentID]) REFERENCES [Learn].[Student] ([StudentID]),
	CONSTRAINT FK_Dismissal_RelativeID FOREIGN KEY ([RelativeID]) REFERENCES [Learn].[Relative] ([RelativeID]),
)
GO
CREATE INDEX IX_Dismissal_CreatedOn_SearchTerms
ON [Learn].[Dismissal] ([CreatedOn] DESC, [SearchTerms])
INCLUDE ([DismissalID], [ClassroomID], [StudentID], [RelativeID], [DismissedOn])
WHERE [DismissedOn] IS NULL;
