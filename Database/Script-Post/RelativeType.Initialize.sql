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
) AS Source ([RelativeTypeID], [Name], [Uid])
ON Target.[RelativeTypeID] = Source.[RelativeTypeID]
WHEN NOT MATCHED THEN
	INSERT ([RelativeTypeID], [Name], [Uid]) 
	VALUES (Source.[RelativeTypeID], Source.[Name], Source.[Uid])
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;

PRINT 'RelativeType loaded.'