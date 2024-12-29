PRINT 'Initializing NavigationItem...'

DECLARE @Label VARCHAR(50),
		@TagLevel1 VARCHAR(50),
		@TagLevel2 VARCHAR(50),
		@ParentId INTEGER,
		@AppType INTEGER = 0 -- EnterpriseManager

-- 1.0.0
SET @Label = 'Dashboard'
SET @TagLevel1 = @Label
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel1 AND AppType = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Tag], [ParentId], [AppType])
		VALUES(@Label, CONVERT(INT, 0xF246), @TagLevel1, NULL, @AppType)
	END
-- 2.0.0
SET @Label = 'Records'
SET @TagLevel1 = @Label
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel1 AND AppType = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Tag], [ParentId], [AppType])
		VALUES(@Label, CONVERT(INT, 0xE838), @TagLevel1, NULL, @AppType)
	END
-- 2.1.0
SET @Label = 'Patients'
SET @TagLevel2 = @Label
SELECT @ParentId = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel1 AND AppType = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel2 AND AppType = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Tag], [ParentId], [AppType])
		VALUES(@Label, CONVERT(INT, 0xE902), @TagLevel2, @ParentId, @AppType)
	END
-- 2.2.0
SET @Label = 'Holydays'
SET @TagLevel2 = @Label
SELECT @ParentId = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel1 AND AppType = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel2 AND AppType = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Tag], [ParentId], [AppType])
		VALUES(@Label, CONVERT(INT, 0xE787), @TagLevel2, @ParentId, @AppType)
	END
-- 3.0.0
SET @Label = 'Operations'
SET @TagLevel1 = @Label
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel1 AND AppType = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Tag], [ParentId], [AppType])
		VALUES(@Label, CONVERT(INT, 0xE912), @TagLevel1, NULL, @AppType)
	END
-- 3.1.0
SET @Label = 'Scheduling'
SET @TagLevel2 = @Label
SELECT @ParentId = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel1 AND AppType = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel2 AND AppType = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Tag], [ParentId], [AppType])
		VALUES(@Label, CONVERT(INT, 0xEC92), @TagLevel2, @ParentId, @AppType)
	END
-- 3.2.0
SET @Label = 'Stock'
SET @TagLevel2 = @Label
SELECT @ParentId = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel1 AND AppType = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel2 AND AppType = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Tag], [ParentId], [AppType])
		VALUES(@Label, CONVERT(INT, 0xE7B8), @TagLevel2, @ParentId, @AppType)
	END
-- 4.0.0
SET @Label = 'Activity Log'
SET @TagLevel1 = 'AppLogs'
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel1 AND AppType = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Tag], [ParentId], [AppType])
		VALUES(@Label, CONVERT(INT, 0xE9D9), @TagLevel1, NULL, @AppType)
	END
-- 99.0.0
SET @Label = 'Administration'
SET @TagLevel1 = @Label
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel1 AND AppType = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Tag], [ParentId], [AppType])
		VALUES(@Label, CONVERT(INT, 0xE7EF), @TagLevel1, NULL, @AppType)
	END
-- 99.1.0
SET @Label = 'Permissions'
SET @TagLevel2 = @Label
SELECT @ParentId = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel1 AND AppType = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel2 AND AppType = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Tag], [ParentId], [AppType])
		VALUES(@Label, CONVERT(INT, 0xE8D7), @TagLevel2, @ParentId, @AppType)
	END
-- 99.2.0
SET @Label = 'Roles'
SET @TagLevel2 = @Label
SELECT @ParentId = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel1 AND AppType = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel2 AND AppType = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Tag], [ParentId], [AppType])
		VALUES(@Label, CONVERT(INT, 0xEE57), @TagLevel2, @ParentId, @AppType)
	END
-- 99.3.0
SET @Label = 'Users'
SET @TagLevel2 = @Label
SELECT @ParentId = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel1 AND AppType = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Tag] = @TagLevel2 AND AppType = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Tag], [ParentId], [AppType])
		VALUES(@Label, CONVERT(INT, 0xE7EE), @TagLevel2, @ParentId, @AppType)
	END

PRINT 'NavigationItem loaded.'