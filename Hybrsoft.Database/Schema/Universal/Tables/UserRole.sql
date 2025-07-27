CREATE TABLE [Universal].[UserRole]
(
	[UserRoleID] BIGINT NOT NULL PRIMARY KEY,
	[UserID] BIGINT NOT NULL,
	[RoleID] BIGINT NOT NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(100) NULL,
	CONSTRAINT FK_UserRole_UserID FOREIGN KEY ([UserID]) REFERENCES [Universal].[User] ([UserID]) ON DELETE CASCADE,
	CONSTRAINT FK_UserRole_RoleID FOREIGN KEY ([RoleID]) REFERENCES [Universal].[Role] ([RoleID]),
	CONSTRAINT UQ_UserRole_UserID_RoleID UNIQUE ([UserID], [RoleID])
)
GO
CREATE INDEX IX_UserRole_UserID_RoleID_SearchTerms
ON [Universal].[UserRole] ([UserID], [RoleID], [SearchTerms]);
