CREATE TABLE [Learn].[Classroom]
(
	[ClassroomId] BIGINT NOT NULL PRIMARY KEY,
	[Name] NVARCHAR(150) NOT NULL,
	[Year] SMALLINT NOT NULL,
	[MinimumYear] SMALLINT NOT NULL,
	[MaximumYear] SMALLINT NOT NULL,
	[EducationLevel] SMALLINT NOT NULL,
	[MinimumEducationLevel] SMALLINT NOT NULL,
	[MaximumEducationLevel] SMALLINT NOT NULL,
	[ScheduleTypeId] SMALLINT NOT NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(200) NULL,
	CONSTRAINT UQ_Classroom_NameYearScheduleTypeId UNIQUE ([Name], [Year], [ScheduleTypeId]),
	CONSTRAINT FK_Classroom_ScheduleTypeId FOREIGN KEY ([ScheduleTypeId]) REFERENCES [Learn].[ScheduleType] ([ScheduleTypeId]),
)
GO
CREATE INDEX IX_Classroom_Year_Name ON [Learn].[Classroom] ([Year] DESC, [Name]);
GO
CREATE INDEX IX_Classroom_SearchTerms ON [Learn].[Classroom] ([SearchTerms]);
