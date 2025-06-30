CREATE TABLE [Universal].[AppLog]
(
	[AppLogID] BIGINT NOT NULL PRIMARY KEY IDENTITY,
	[IsRead] BIT NULL,
	[CreateOn] DATETIMEOFFSET NOT NULL,
	[User] NVARCHAR(50) NOT NULL,
	[Type] INT NOT NULL,
	[Source] NVARCHAR(50) NOT NULL,
	[Action] NVARCHAR(50) NOT NULL,
	[Message] NVARCHAR(400) NOT NULL,
	[Description] NVARCHAR(4000) NOT NULL,
	[AppType] INT NOT NULL
)
GO
CREATE INDEX IX_AppLog_AppType_IsRead ON [Universal].[AppLog] ([AppType], [IsRead]);
GO
CREATE INDEX IX_AppLog_AppType_CreateOn ON [Universal].[AppLog] ([AppType], [CreateOn]);
