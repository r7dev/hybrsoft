CREATE TABLE [Universal].[RolePermission]
(
	[RolePermissionID] BIGINT NOT NULL PRIMARY KEY,
	[RoleID] BIGINT NOT NULL,
	[PermissionID] BIGINT NOT NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(100) NULL,
	CONSTRAINT FK_RolePermission_RoleID FOREIGN KEY ([RoleID]) REFERENCES [Universal].[Role] ([RoleID]) ON DELETE CASCADE,
	CONSTRAINT FK_RolePermission_PermissionID FOREIGN KEY ([PermissionID]) REFERENCES [Universal].[Permission] ([PermissionID]),
	CONSTRAINT UQ_RolePermission_RoleID_PermissionID UNIQUE ([RoleID], [PermissionID])
)
GO
CREATE INDEX IX_RolePermission_RoleID_PermissionID_SearchTerms
ON [Universal].[RolePermission] ([RoleID], [PermissionID], [SearchTerms]);
