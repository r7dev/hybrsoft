PRINT 'Initializing NavigationItem...'

DECLARE @TagLevel1 VARCHAR(50),
		@TagLevel2 VARCHAR(50),
		@ParentId INTEGER,
		@AppType INTEGER = 0 -- EnterpriseManager

-- 1.0.0
SET @TagLevel1 = 'Dashboard'
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel1)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Tag], [ParentId], [AppType])
		VALUES(@TagLevel1, CONVERT(INT, 0xF246), @TagLevel1, NULL, @AppType)
	END
-- 2.0.0
SET @TagLevel1 = 'Records'
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel1)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Tag], [ParentId], [AppType])
		VALUES(@TagLevel1, CONVERT(INT, 0xE838), @TagLevel1, NULL, @AppType)
	END
-- 2.1.0
SET @TagLevel2 = 'Patients'
SELECT @ParentId = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel1
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel2)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Tag], [ParentId], [AppType])
		VALUES(@TagLevel2, CONVERT(INT, 0xE902), @TagLevel2, @ParentId, @AppType)
	END
-- 2.2.0
SET @TagLevel2 = 'Users'
SELECT @ParentId = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel1
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel2)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Tag], [ParentId], [AppType])
		VALUES(@TagLevel2, CONVERT(INT, 0xEF58), @TagLevel2, @ParentId, @AppType)
	END
-- 3.0.0
SET @TagLevel1 = 'Operations'
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel1)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Tag], [ParentId], [AppType])
		VALUES(@TagLevel1, CONVERT(INT, 0xE912), @TagLevel1, NULL, @AppType)
	END
-- 3.1.0
SET @TagLevel2 = 'Scheduling'
SELECT @ParentId = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel1
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel2)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Tag], [ParentId], [AppType])
		VALUES(@TagLevel2, CONVERT(INT, 0xEC92), @TagLevel2, @ParentId, @AppType)
	END
-- 3.2.0
SET @TagLevel2 = 'Stock'
SELECT @ParentId = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel1
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel2)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Tag], [ParentId], [AppType])
		VALUES(@TagLevel2, CONVERT(INT, 0xE7B8), @TagLevel2, @ParentId, @AppType)
	END

PRINT 'NavigationItem loaded.'