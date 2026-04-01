PRINT 'Initializing Role...'

DECLARE @Name VARCHAR(50) = 'Administrator'

IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[Role] WHERE [Name] = @Name)
	BEGIN
		INSERT INTO [Universal].[Role] ([RoleID], [Name], [CreatedOn], [LastModifiedOn], [SearchTerms])
		VALUES (20250101, @Name, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), '20250101 administrator')
	END

PRINT 'Role loaded.'