CREATE TABLE [Universal].[NavigationItem]
(
	[NavigationItemId] INT NOT NULL PRIMARY KEY IDENTITY,
	[Label] NVARCHAR(50) NOT NULL,
	[Icon] INT NULL,
	[Uid] NVARCHAR(50) NULL,
	[ViewModel] NVARCHAR(50) NULL,
	[ParentId] INT NULL,
	[AppType] INT NOT NULL,
	CONSTRAINT FK_NavigationItem_ParentId FOREIGN KEY ([ParentId]) REFERENCES [Universal].[NavigationItem] ([NavigationItemId])
)
GO
CREATE INDEX IX_NavigationItem_AppType ON [Universal].[NavigationItem] (AppType);
