PRINT 'Initializing Role Permission...'

DECLARE @Name VARCHAR(50) = 'Administrator',
		@DisplayName VARCHAR(100) = 'SecurityAdministration'

IF NOT EXISTS(SELECT TOP 1 1
			  FROM [Universal].[RolePermission] rp
				INNER JOIN [Universal].[Role] r
					ON r.[RoleID] = rp.[RoleID]
				INNER JOIN [Universal].[Permission] p
					ON p.[PermissionID] = rp.[PermissionID]
			  WHERE r.[Name] = @Name
				AND p.[DisplayName] = @DisplayName)
	BEGIN
		DECLARE @RoleID BIGINT = (SELECT TOP 1 [RoleID]
								  FROM [Universal].[Role]
								  WHERE [Name] = @Name),
				@PermissionID BIGINT = (SELECT TOP 1 [PermissionID]
										FROM [Universal].[Permission]
										WHERE [DisplayName] = @DisplayName)

		IF (@RoleID IS NOT NULL AND @PermissionID IS NOT NULL)
			BEGIN
			INSERT INTO [Universal].[RolePermission] ([RolePermissionID],
													  [RoleID],
													  [PermissionID],
													  [CreatedOn],
													  [LastModifiedOn],
													  [SearchTerms])
			VALUES (20250101,
					@RoleID,
					@PermissionID,
					SYSDATETIMEOFFSET(),
					SYSDATETIMEOFFSET(),
					CAST(@PermissionID AS VARCHAR) + ' security administration')
			END
	END

PRINT 'Role Permission loaded.'