CREATE TABLE [Universal].[Menu]
(
	[MenuId] INT NOT NULL PRIMARY KEY IDENTITY,
	[Nome] VARCHAR(50) NOT NULL,
	[Icone] INT NULL,
	[SuperiorId] INT NULL,
	CONSTRAINT [FK_Menu_SuperiorId] FOREIGN KEY ([SuperiorId]) REFERENCES [Universal].[Menu] ([MenuId])
)
