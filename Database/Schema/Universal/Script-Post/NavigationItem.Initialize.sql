PRINT 'Initializing NavigationItem...'

DECLARE @Label VARCHAR(50),
		@LabelLevel2 VARCHAR(50),
		@ParentId INTEGER,
		@AppType INTEGER = 0, -- EnterpriseManager
		@IsRewrite BIT = 1

IF @IsRewrite = 1 AND EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem])
	BEGIN
		PRINT 'Deleting NavigationItem...'
		DELETE FROM [Universal].[NavigationItem]
		PRINT 'NavigationItem Deleted.'
		-- Reset identity incremental
		PRINT 'Reseting identity incremental'
		DBCC CHECKIDENT ('Universal.NavigationItem', RESEED, 0)
		PRINT 'Identity incremental reseted.'
	END

-- 1.0.0
SET @Label = 'Dashboard'
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentId], [AppType])
		VALUES(@Label, CONVERT(INT, 0xF246), NULL, 'DashboardViewModel', NULL, @AppType)
	END
-- 2.0.0
SET @Label = 'Records'
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentId], [AppType])
		VALUES(@Label, CONVERT(INT, 0xE838), NULL, NULL, NULL, @AppType)
	END
-- 2.1.0
SET @LabelLevel2 = 'Students'
SELECT @ParentId = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @LabelLevel2 AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentId], [AppType])
		VALUES(@LabelLevel2, CONVERT(INT, 0xE716), 'NavigationItem_Students', 'StudentsViewModel', @ParentId, @AppType)
	END
-- 2.2.0
SET @LabelLevel2 = 'Holydays'
SELECT @ParentId = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @LabelLevel2 AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentId], [AppType])
		VALUES(@LabelLevel2, CONVERT(INT, 0xE787), NULL, NULL, @ParentId, @AppType)
	END
-- 3.0.0
SET @Label = 'Operations'
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentId], [AppType])
		VALUES(@Label, CONVERT(INT, 0xE912), NULL, NULL, NULL, @AppType)
	END
-- 3.1.0
SET @LabelLevel2 = 'Scheduling'
SELECT @ParentId = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @LabelLevel2 AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentId], [AppType])
		VALUES(@LabelLevel2, CONVERT(INT, 0xEC92), NULL, NULL, @ParentId, @AppType)
	END
-- 3.2.0
SET @LabelLevel2 = 'Stock'
SELECT @ParentId = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @LabelLevel2 AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentId], [AppType])
		VALUES(@LabelLevel2, CONVERT(INT, 0xE7B8), NULL, NULL, @ParentId, @AppType)
	END
-- 4.0.0
SET @Label = 'Activity Log'
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentId], [AppType])
		VALUES(@Label, CONVERT(INT, 0xE9D9), 'NavigationItem_Activity_Logs', 'AppLogsViewModel', NULL, @AppType)
	END
-- 99.0.0
SET @Label = 'Administration'
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentId], [AppType])
		VALUES(@Label, CONVERT(INT, 0xE7EF), 'NavigationItem_Administration', NULL, NULL, @AppType)
	END
-- 99.1.0
SET @LabelLevel2 = 'Permissions'
SELECT @ParentId = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @LabelLevel2 AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentId], [AppType])
		VALUES(@LabelLevel2, CONVERT(INT, 0xE8D7), 'NavigationItem_Permissions', 'PermissionsViewModel', @ParentId, @AppType)
	END
-- 99.2.0
SET @LabelLevel2 = 'Roles'
SELECT @ParentId = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @LabelLevel2 AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentId], [AppType])
		VALUES(@LabelLevel2, CONVERT(INT, 0xEE57), 'NavigationItem_Roles', 'RolesViewModel', @ParentId, @AppType)
	END
-- 99.3.0
SET @LabelLevel2 = 'Users'
SELECT @ParentId = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @LabelLevel2 AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentId], [AppType])
		VALUES(@LabelLevel2, CONVERT(INT, 0xE7EE), 'NavigationItem_Users', 'UsersViewModel', @ParentId, @AppType)
	END

PRINT 'NavigationItem loaded.'