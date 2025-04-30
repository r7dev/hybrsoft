PRINT 'Initializing ScheduleType...'

IF NOT EXISTS(SELECT TOP 1 1 FROM [Learn].[ScheduleType])
	BEGIN
		INSERT INTO [Learn].[ScheduleType] ([ScheduleTypeId], [Name], [LanguageTag]) VALUES(1, 'Full-day', 'en-US')
		INSERT INTO [Learn].[ScheduleType] ([ScheduleTypeId], [Name], [LanguageTag]) VALUES(2, 'Integral', 'pt-BR')
		INSERT INTO [Learn].[ScheduleType] ([ScheduleTypeId], [Name], [LanguageTag]) VALUES(3, 'Matutino', 'pt-BR')
		INSERT INTO [Learn].[ScheduleType] ([ScheduleTypeId], [Name], [LanguageTag]) VALUES(4, 'Noturno', 'pt-BR')
		INSERT INTO [Learn].[ScheduleType] ([ScheduleTypeId], [Name], [LanguageTag]) VALUES(5, 'Vespertino', 'pt-BR')
	END

PRINT 'ScheduleType loaded.'