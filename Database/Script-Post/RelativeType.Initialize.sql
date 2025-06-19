PRINT 'Initializing RelativeType...'

MERGE INTO [Learn].[RelativeType] AS Target
USING
(
	VALUES
		(1, 'Parents', 'RelativeType_Parents'),
		(2, 'Siblings', 'RelativeType_Siblings'),
		(3, 'Uncles', 'RelativeType_Uncles'),
		(4, 'Cousins', 'RelativeType_Coursins'),
		(5, 'Grandparents', 'RelativeType_Grandparents'),
		(6, 'Driver', 'RelativeType_Driver'),
		(7, 'Nanny', 'RelativeType_Nanny')
) AS Source ([RelativeTypeId], [Name], [Uid])
ON Target.[RelativeTypeId] = Source.[RelativeTypeId]
WHEN NOT MATCHED THEN
	INSERT ([RelativeTypeId], [Name], [Uid]) 
	VALUES (Source.[RelativeTypeId], Source.[Name], Source.[Uid])
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;

PRINT 'RelativeType loaded.'