IF NOT EXISTS(SELECT TOP 1 1 FROM [sys].[databases] WHERE [name] = 'Hybrsoft')
	BEGIN
		PRINT 'Creating new database Hybrsoft...'
		CREATE DATABASE [Hybrsoft]
		PRINT 'New database Hybrsoft created.'
	END