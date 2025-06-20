﻿CREATE TABLE [Learn].[ClassroomStudent]
(
	[ClassroomStudentId] BIGINT NOT NULL PRIMARY KEY,
	[ClassroomId] BIGINT NOT NULL,
	[StudentId] BIGINT NOT NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(350) NULL,
	[SearchTermsDismissibleStudent] NVARCHAR(350) NULL,
	CONSTRAINT FK_ClassroomStudent_ClassroomId FOREIGN KEY ([ClassroomId]) REFERENCES [Learn].[Classroom] ([ClassroomId]) ON DELETE CASCADE,
	CONSTRAINT FK_ClassroomStudent_StudentId FOREIGN KEY ([StudentId]) REFERENCES [Learn].[Student] ([StudentId]),
	CONSTRAINT UQ_ClassroomStudent_ClassroomId_StudentId UNIQUE ([ClassroomId], [StudentId])
)
GO
CREATE INDEX IX_ClassroomStudent_ClassroomId_StudentId_SearchTerms
ON [Learn].[ClassroomStudent] ([ClassroomId], [StudentId], [SearchTerms]);
GO
CREATE INDEX IX_ClassroomStudent_StudentId_ClassroomId_SearchTermsDismissibleStudent
ON [Learn].[ClassroomStudent] ([StudentId], [ClassroomId], [SearchTermsDismissibleStudent]);
