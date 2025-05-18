PRINT 'Initializing RelativeType...'

MERGE INTO [Learn].[RelativeType] AS Target
USING
(
	VALUES
		(1, 'Parents', 'en-US'),
		(2, 'Siblings', 'en-US'),
		(3, 'Uncles', 'en-US'),
		(4, 'Cousins', 'en-US'),
		(5, 'Grandparents', 'en-US'),
		(6, 'Driver', 'en-US'),
		(7, 'Nanny', 'en-US'),
		(8, 'Pais', 'pt-BR'),
		(9, 'Irmãos', 'pt-BR'),
		(10, 'Tios', 'pt-BR'),
		(11, 'Primos', 'pt-BR'),
		(12, 'Avós', 'pt-BR'),
		(13, 'Motorista', 'pt-BR'),
		(14, 'Babá', 'pt-BR')
) AS Source ([RelativeTypeId], [Name], [LanguageTag])
ON Target.[RelativeTypeId] = Source.[RelativeTypeId]

WHEN NOT MATCHED THEN
	INSERT ([RelativeTypeId], [Name], [LanguageTag]) 
	VALUES (Source.[RelativeTypeId], Source.[Name], Source.[LanguageTag])

WHEN NOT MATCHED BY SOURCE THEN
	DELETE;

PRINT 'RelativeType loaded.'