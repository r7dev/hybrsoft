CREATE TABLE [Universal].[CompanyUser]
(
	[CompanyUserID] BIGINT NOT NULL PRIMARY KEY,
	[CompanyID] BIGINT NOT NULL,
	[UserID] BIGINT NOT NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(100) NULL,
	CONSTRAINT FK_CompanyUser_CompanyID FOREIGN KEY ([CompanyID]) REFERENCES [Universal].[Company] ([CompanyID]) ON DELETE CASCADE,
	CONSTRAINT FK_CompanyUser_UserID FOREIGN KEY ([UserID]) REFERENCES [Universal].[User] ([UserID])
)
