/*
 Modelo de Script Pré-Implantação							
--------------------------------------------------------------------------------------
 Este arquivo contém instruções SQL que serão executadas antes do script de compilação.	
 Use a sintaxe SQLCMD para incluir um arquivo no script pré-implantação.			
 Exemplo:      :r .\myfile.sql								
 Use a sintaxe SQLCMD para referenciar uma variável no script pré-implantação.		
 Exemplo:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
DECLARE @Name VARCHAR(50) = 'EnterpriseManager'

USE [master]
IF NOT EXISTS(SELECT TOP 1 1 FROM [sys].[server_principals] WHERE [type] = 'S' AND [name] = @Name)
	BEGIN
		PRINT 'Creating new login ' + @Name + '...'
		USE [hybrsoft];
		CREATE LOGIN EnterpriseManager WITH PASSWORD = 0x0200B63661A35F607A6466A55A76004509BF7C4FBF83E0C8F5A908B7A03D2BA85B18DA303051EF2928693F1C8BE367E0E37A6B0966CD2D6255DE716B598FFAA2C48E8E1B1755 HASHED,
		DEFAULT_DATABASE = [hybrsoft];
		PRINT 'New login ' + @Name + ' created with success.'
	END
ELSE
	BEGIN
		PRINT 'Login ' + @Name + ' has already been created.'
	END

USE [hybrsoft]
IF NOT EXISTS(SELECT TOP 1 1 FROM [sys].[database_principals] WHERE [type] = 'S' AND [name] = @Name)
	BEGIN
		PRINT 'Creating new user ' + @Name + '...'
		CREATE USER EnterpriseManager FOR LOGIN EnterpriseManager;
		EXEC sp_addrolemember 'db_datareader', @Name
		EXEC sp_addrolemember 'db_datawriter', @Name
		PRINT 'New user ' + @Name + ' created with success.'
	END
ELSE
	BEGIN
		PRINT 'User ' + @Name + ' has already been created.'
	END
