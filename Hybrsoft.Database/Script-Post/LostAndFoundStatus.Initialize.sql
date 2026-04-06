PRINT 'Initializing LostAndFoundStatus...'

MERGE INTO [Learn].[LostAndFoundStatus] AS Target
USING
(
	VALUES
		(1, 'Pending', 'LostAndFoundStatus_Pending'),
		(2, 'Claimed', 'LostAndFoundStatus_Claimed'),
		(3, 'Donated', 'LostAndFoundStatus_Donated')
) AS Source ([LostAndFoundStatusID], [Name], [Uid])
ON Target.[LostAndFoundStatusID] = Source.[LostAndFoundStatusID]
WHEN NOT MATCHED THEN
	INSERT ([LostAndFoundStatusID], [Name], [Uid])
	VALUES (Source.[LostAndFoundStatusID], Source.[Name], Source.[Uid])
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;

PRINT 'LostAndFoundStatus loaded.'