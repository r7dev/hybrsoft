﻿CREATE TABLE [Universal].[AppLog]
(
	[AppLogId] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
	[IsRead] BIT NULL,
	[CreateOn] DATETIMEOFFSET NOT NULL,
	[User] VARCHAR(50) NOT NULL,
	[Type] INT NOT NULL,
	[Source] VARCHAR(50) NOT NULL,
	[Action] VARCHAR(50) NOT NULL,
	[Message] VARCHAR(400) NOT NULL,
	[Description] VARCHAR(4000) NOT NULL,
	[AppType] INT NOT NULL
)
