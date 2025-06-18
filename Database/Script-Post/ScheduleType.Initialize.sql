PRINT 'Initializing ScheduleType...'

MERGE INTO [Learn].[ScheduleType] AS Target
USING
(
	VALUES
		(1, 'Full-day', 'ScheduleType_Fullday'),
		(2, 'Morning', 'ScheduleType_Morning'),
		(3, 'Afternoon', 'ScheduleType_Afternoon'),
		(4, 'Nocturnal', 'ScheduleType_Nocturnal')
) AS Source ([ScheduleTypeId], [Name], [Uid])
ON Target.[ScheduleTypeId] = Source.[ScheduleTypeId]
WHEN NOT MATCHED THEN
	INSERT ([ScheduleTypeId], [Name], [Uid]) 
	VALUES (Source.[ScheduleTypeId], Source.[Name], Source.[Uid])
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;

PRINT 'ScheduleType loaded.'