CREATE TABLE [Universal].[UserRole]
(
	[UserRoleId] BIGINT NOT NULL PRIMARY KEY,
	[UserId] BIGINT NOT NULL,
	[RoleId] BIGINT NOT NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(100) NULL,
	CONSTRAINT FK_UserRole_UserId FOREIGN KEY ([UserId]) REFERENCES [Universal].[User] ([UserId]) ON DELETE CASCADE,
	CONSTRAINT FK_UserRole_RoleId FOREIGN KEY ([RoleId]) REFERENCES [Universal].[Role] ([RoleId]),
	CONSTRAINT UQ_UserRole_UserId_RoleId UNIQUE ([UserId], [RoleId])
)
GO
CREATE INDEX IX_UserRole_UserId_RoleId ON [Universal].[UserRole] ([UserId], [RoleId]);
GO
CREATE INDEX IX_UserRole_SearchTerms ON [Universal].[UserRole] ([SearchTerms]);
