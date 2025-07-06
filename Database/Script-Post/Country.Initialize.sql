PRINT 'Initializing Countries...'

MERGE INTO [Universal].[Country] AS Target
USING
(
	VALUES
		(1, 'United States', 'Country_UnitedStates'),
		(55, 'Brazil', 'Country_Brazil')
) AS Source ([CountryID], [Name], [Uid])
ON Target.[CountryID] = Source.[CountryID]
WHEN NOT MATCHED THEN
	INSERT ([CountryID], [Name], [Uid]) 
	VALUES (Source.[CountryID], Source.[Name], Source.[Uid])
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;

PRINT 'Countries loaded.'