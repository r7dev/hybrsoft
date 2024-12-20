﻿CREATE TABLE [Universal].[User]
(
	[UserId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
	[FirstName] VARCHAR(50) NOT NULL, 
	[LastName] VARCHAR(50) NOT NULL, 
	[Email] VARCHAR(150) NOT NULL UNIQUE,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NOT NULL,
	[SearchTerms] VARCHAR(MAX) NULL
)
GO
CREATE INDEX IX_User_FirstName ON [Universal].[User] (FirstName);
GO
CREATE INDEX IX_User_SearchTerms ON [Universal].[User] (SearchTerms);
