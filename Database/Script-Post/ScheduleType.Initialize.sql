PRINT 'Initializing ScheduleType...'

MERGE INTO [Learn].[ScheduleType] AS Target
USING
(
	VALUES
		(1, 'Full-day', 'ScheduleType_Fullday'),
		(2, 'Morning', 'ScheduleType_Morning'),
		(3, 'Afternoon', 'ScheduleType_Afternoon'),
		(4, 'Nocturnal', 'ScheduleType_Nocturnal')
) AS Source ([ScheduleTypeID], [Name], [Uid])
ON Target.[ScheduleTypeID] = Source.[ScheduleTypeID]
WHEN NOT MATCHED THEN
	INSERT ([ScheduleTypeID], [Name], [Uid]) 
	VALUES (Source.[ScheduleTypeID], Source.[Name], Source.[Uid])
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;

PRINT 'ScheduleType loaded.'