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

PRINT 'Inicio do script pós-implantação'
PRINT 'Configurando menu'

DECLARE @NomeNivel1 VARCHAR(50),
		@NomeNivel2 VARCHAR(50),
		@SuperiorId INTEGER
-- 1.0.0
SET @NomeNivel1 = 'Dashboard'
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[Menu] WHERE [Nome] = @NomeNivel1)
	BEGIN
		INSERT INTO [Universal].[Menu] ([Nome], [Icone], [SuperiorId]) VALUES(@NomeNivel1, CONVERT(INT, 0xF246), NULL)
	END
-- 2.0.0
SET @NomeNivel1 = 'Cadastros'
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[Menu] WHERE [Nome] = @NomeNivel1)
	BEGIN
		INSERT INTO [Universal].[Menu] ([Nome], [Icone], [SuperiorId]) VALUES(@NomeNivel1, CONVERT(INT, 0xE838), NULL)
	END
-- 2.1.0
SET @NomeNivel2 = 'Pacientes'
SELECT @SuperiorId = [MenuId] FROM [Universal].[Menu] WHERE [Nome] = @NomeNivel1
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[Menu] WHERE [Nome] = @NomeNivel2)
	BEGIN
		INSERT INTO [Universal].[Menu] ([Nome], [Icone], [SuperiorId]) VALUES(@NomeNivel2, CONVERT(INT, 0xE902), @SuperiorId)
	END
-- 2.2.0
SET @NomeNivel2 = 'Usuários'
SELECT @SuperiorId = [MenuId] FROM [Universal].[Menu] WHERE [Nome] = @NomeNivel1
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[Menu] WHERE [Nome] = @NomeNivel2)
	BEGIN
		INSERT INTO [Universal].[Menu] ([Nome], [Icone], [SuperiorId]) VALUES(@NomeNivel2, CONVERT(INT, 0xEF58), @SuperiorId)
	END
-- 3.0.0
SET @NomeNivel1 = 'Operação'
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[Menu] WHERE [Nome] = @NomeNivel1)
	BEGIN
		INSERT INTO [Universal].[Menu] ([Nome], [Icone], [SuperiorId]) VALUES(@NomeNivel1, CONVERT(INT, 0xE912), NULL)
	END
-- 3.1.0
SET @NomeNivel2 = 'Agendamento'
SELECT @SuperiorId = [MenuId] FROM [Universal].[Menu] WHERE [Nome] = @NomeNivel1
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[Menu] WHERE [Nome] = @NomeNivel2)
	BEGIN
		INSERT INTO [Universal].[Menu] ([Nome], [Icone], [SuperiorId]) VALUES(@NomeNivel2, CONVERT(INT, 0xEC92), @SuperiorId)
	END
-- 3.2.0
SET @NomeNivel2 = 'Estoque'
SELECT @SuperiorId = [MenuId] FROM [Universal].[Menu] WHERE [Nome] = @NomeNivel1
IF NOT EXISTS(SELECT TOP 1 1 FROM [Universal].[Menu] WHERE [Nome] = @NomeNivel2)
	BEGIN
		INSERT INTO [Universal].[Menu] ([Nome], [Icone], [SuperiorId]) VALUES(@NomeNivel2, CONVERT(INT, 0xE7B8), @SuperiorId)
	END

PRINT 'Menu configurado'
PRINT 'Fim do script pós-implantação'