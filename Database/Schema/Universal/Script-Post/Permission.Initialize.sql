/*
Modelo de Script Pós-Implantação							
--------------------------------------------------------------------------------------
 Este arquivo contém instruções SQL que serão acrescentadas ao script de compilação.		
 Use sintaxe SQLCMD para incluir um arquivo no script pós-implantação.			
 Exemplo:      :r .\myfile.sql								
 Use sintaxe SQLCMD para referenciar uma variável no script pós-implantação.		
 Exemplo:      :setvar TableName MyTable							
			   SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
PRINT 'Initializing Permission...'

DECLARE @Name VARCHAR(50) = 'SecurityAdministration'

IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[Permission] WHERE [Name] = @Name)
	BEGIN
		INSERT INTO [Universal].[Permission] ([PermissionId], [Name], [DisplayName], [Description], [IsEnabled], [CreatedOn], [LastModifiedOn], [SearchTerms])
		VALUES (20250101, @Name, 'Security Administration', 'Provides the ability to manager security for the application', 0, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), '20250101 securityadministration security administration provides the ability to manager security for the application')
	END

PRINT 'Permission loaded.'