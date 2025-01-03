﻿CREATE TABLE [Universal].[Category]
(
	[CategoryId] INT NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(50) NOT NULL,
	[Description] VARCHAR(400) NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] VARCHAR(500) NULL
)
