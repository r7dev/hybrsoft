PRINT 'Initializing User Role...'

DECLARE @Email VARCHAR(150) = 'ricardo.machado@outlook.com.br',
		@Name VARCHAR(50) = 'Administrator'

IF NOT EXISTS(SELECT TOP 1 1
			  FROM [Universal].[UserRole] ur
				INNER JOIN [Universal].[User] u
					ON u.[UserID] = ur.[UserID]
				INNER JOIN [Universal].[Role] r
					ON r.[RoleID] = ur.[RoleID]
			  WHERE u.[Email] = @Email
				AND r.[Name] = @Name)
	BEGIN
		DECLARE @UserID BIGINT = (SELECT TOP 1 [UserID]
								  FROM [Universal].[User]
								  WHERE [Email] = @Email),
				@RoleID BIGINT = (SELECT TOP 1 [RoleID]
								  FROM [Universal].[Role]
								  WHERE [Name] = @Name)

		IF (@UserID IS NOT NULL AND @RoleID IS NOT NULL)
			BEGIN
			INSERT INTO [Universal].[UserRole] ([UserRoleID],
												[UserID],
												[RoleID],
												[CreatedOn],
												[LastModifiedOn],
												[SearchTerms])
			VALUES (20250101,
					@UserID,
					@RoleID,
					SYSDATETIMEOFFSET(),
					SYSDATETIMEOFFSET(),
					CAST(@RoleID AS VARCHAR) + ' administrator')
			END
	END

PRINT 'User Role loaded.'