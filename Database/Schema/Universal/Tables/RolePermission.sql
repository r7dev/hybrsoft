CREATE TABLE [Universal].[RolePermission]
(
	[RolePermissionId] BIGINT NOT NULL PRIMARY KEY,
	[RoleId] BIGINT NOT NULL,
	[PermissionId] BIGINT NOT NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(100) NULL,
	CONSTRAINT FK_RolePermission_RoleId FOREIGN KEY ([RoleId]) REFERENCES [Universal].[Role] ([RoleId]) ON DELETE CASCADE,
	CONSTRAINT FK_RolePermission_PermissionId FOREIGN KEY ([PermissionId]) REFERENCES [Universal].[Permission] ([PermissionId]),
	CONSTRAINT UQ_RolePermission_RoleId_PermissionId UNIQUE ([RoleId], [PermissionId])
)
GO
CREATE INDEX IX_RolePermission_RoleId_PermissionId_SearchTerms
ON [Universal].[RolePermission] ([RoleId], [PermissionId], [SearchTerms]);
