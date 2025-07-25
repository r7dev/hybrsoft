﻿CREATE TABLE [Universal].[User]
(
	[UserID] BIGINT NOT NULL PRIMARY KEY,
	[FirstName] NVARCHAR(50) NOT NULL,
	[MiddleName] NVARCHAR(50) NULL,
	[LastName] NVARCHAR(50) NOT NULL,
	[Email] NVARCHAR(150) NOT NULL,
	[Password] NVARCHAR(100) NOT NULL,
	[PasswordLength] INT NOT NULL DEFAULT 0,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(300) NULL,
	CONSTRAINT UQ_User_Email UNIQUE ([Email]),
)
GO
CREATE INDEX IX_User_FirstName_SearchTerms
ON [Universal].[User] ([FirstName], [SearchTerms])
INCLUDE ([LastName], [Email], [LastModifiedOn]);
