PRINT 'Initializing NavigationItem...'

DECLARE @Label VARCHAR(50),
		@LabelLevel2 VARCHAR(50),
		@ParentID INTEGER,
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
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentID], [AppType])
		VALUES(@Label, CONVERT(INT, 0xF246), NULL, 'DashboardViewModel', NULL, @AppType)
	END
-- 2.0.0
SET @Label = 'Records'
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentID], [AppType])
		VALUES(@Label, CONVERT(INT, 0xE838), 'NavigationItem_Records', NULL, NULL, @AppType)
	END
-- 2.1.0
SET @LabelLevel2 = 'Relatives'
SELECT @ParentID = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @LabelLevel2 AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentID], [AppType])
		VALUES(@LabelLevel2, CONVERT(INT, 0xEBDA), 'NavigationItem_Relatives', 'RelativesViewModel', @ParentID, @AppType)
	END
-- 2.2.0
SET @LabelLevel2 = 'Students'
SELECT @ParentID = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @LabelLevel2 AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentID], [AppType])
		VALUES(@LabelLevel2, CONVERT(INT, 0xE716), 'NavigationItem_Students', 'StudentsViewModel', @ParentID, @AppType)
	END
-- 2.3.0
SET @LabelLevel2 = 'Classrooms'
SELECT @ParentID = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @LabelLevel2 AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentID], [AppType])
		VALUES(@LabelLevel2, CONVERT(INT, 0xE902), 'NavigationItem_Classrooms', 'ClassroomsViewModel', @ParentID, @AppType)
	END
-- 2.4.0
SET @LabelLevel2 = 'Companies'
SELECT @ParentID = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @LabelLevel2 AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentID], [AppType])
		VALUES(@LabelLevel2, CONVERT(INT, 0xE731), 'NavigationItem_Companies', 'CompaniesViewModel', @ParentID, @AppType)
	END
-- 3.0.0
SET @Label = 'Operations'
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentID], [AppType])
		VALUES(@Label, CONVERT(INT, 0xE912), 'NavigationItem_Operations', NULL, NULL, @AppType)
	END
-- 3.1.0
SET @LabelLevel2 = 'Dismissible Students'
SELECT @ParentID = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @LabelLevel2 AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentID], [AppType])
		VALUES(@LabelLevel2, CONVERT(INT, 0xE789), 'NavigationItem_DismissibleStudents', 'DismissibleStudentsViewModel', @ParentID, @AppType)
	END
-- 3.2.0
SET @LabelLevel2 = 'Dismissals'
SELECT @ParentID = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @LabelLevel2 AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentID], [AppType])
		VALUES(@LabelLevel2, CONVERT(INT, 0xEFA9), 'NavigationItem_Dismissals', 'DismissalsViewModel', @ParentID, @AppType)
	END
-- 4.0.0
SET @Label = 'Activity Log'
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentID], [AppType])
		VALUES(@Label, CONVERT(INT, 0xE9D9), 'NavigationItem_Activity_Logs', 'AppLogsViewModel', NULL, @AppType)
	END
-- 99.0.0
SET @Label = 'Administration'
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentID], [AppType])
		VALUES(@Label, CONVERT(INT, 0xE7EF), 'NavigationItem_Administration', NULL, NULL, @AppType)
	END
-- 99.1.0
SET @LabelLevel2 = 'Permissions'
SELECT @ParentID = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @LabelLevel2 AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentID], [AppType])
		VALUES(@LabelLevel2, CONVERT(INT, 0xE8D7), 'NavigationItem_Permissions', 'PermissionsViewModel', @ParentID, @AppType)
	END
-- 99.2.0
SET @LabelLevel2 = 'Roles'
SELECT @ParentID = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @LabelLevel2 AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentID], [AppType])
		VALUES(@LabelLevel2, CONVERT(INT, 0xEE57), 'NavigationItem_Roles', 'RolesViewModel', @ParentID, @AppType)
	END
-- 99.3.0
SET @LabelLevel2 = 'Users'
SELECT @ParentID = [NavigationItemId] FROM [Universal].[NavigationItem] WHERE [Label] = @Label AND [AppType] = @AppType
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[NavigationItem] WHERE [Label] = @LabelLevel2 AND [AppType] = @AppType)
	BEGIN
		INSERT INTO [Universal].[NavigationItem] ([Label], [Icon], [Uid], [ViewModel], [ParentID], [AppType])
		VALUES(@LabelLevel2, CONVERT(INT, 0xE7EE), 'NavigationItem_Users', 'UsersViewModel', @ParentID, @AppType)
	END

PRINT 'NavigationItem loaded.'