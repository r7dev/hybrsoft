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
:r DataBaseUsersAdd.sql