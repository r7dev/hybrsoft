CREATE TABLE [Learn].[ClassroomStudent]
(
	[ClassroomStudentID] BIGINT NOT NULL PRIMARY KEY,
	[ClassroomID] BIGINT NOT NULL,
	[StudentID] BIGINT NOT NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(350) NULL,
	[SearchTermsDismissibleStudent] NVARCHAR(350) NULL,
	CONSTRAINT FK_ClassroomStudent_ClassroomID FOREIGN KEY ([ClassroomID]) REFERENCES [Learn].[Classroom] ([ClassroomID]) ON DELETE CASCADE,
	CONSTRAINT FK_ClassroomStudent_StudentID FOREIGN KEY ([StudentID]) REFERENCES [Learn].[Student] ([StudentID]),
	CONSTRAINT UQ_ClassroomStudent_ClassroomId_StudentID UNIQUE ([ClassroomID], [StudentID])
)
GO
CREATE INDEX IX_ClassroomStudent_ClassroomID_StudentID_SearchTerms
ON [Learn].[ClassroomStudent] ([ClassroomID], [StudentID], [SearchTerms]);
GO
CREATE INDEX IX_ClassroomStudent_StudentID_ClassroomID_SearchTermsDismissibleStudent
ON [Learn].[ClassroomStudent] ([StudentID], [ClassroomID], [SearchTermsDismissibleStudent]);
