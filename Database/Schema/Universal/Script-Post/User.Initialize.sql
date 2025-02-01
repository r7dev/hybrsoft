PRINT 'Initializing User...'

DECLARE @Email VARCHAR(150) = 'ricardo.machado@outlook.com.br'

IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[User] WHERE [Email] = @Email)
	BEGIN
		INSERT INTO [Universal].[User] ([UserId], [FirstName], [MiddleName], [LastName], [Email], [Password], [PasswordLength], [CreatedOn], [LastModifiedOn], [SearchTerms])
		VALUES (20250101, 'Ricardo', 'Franco', 'Machado', @Email, 'F72BB7F1A193404C2E19A73BCD1F8F01DA011BDFA987A33D30D032F2C9B0F5F9-AE7C1CDFE3BB9BD5FC2D8664BBECAF21', 8, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), '20250101 ricardo machado ricardo.machado@outlook.com.br')
	END

PRINT 'User loaded.'