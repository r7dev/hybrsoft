CREATE TABLE [Universal].[NavigationItem]
(
	[NavigationItemID] INT NOT NULL PRIMARY KEY IDENTITY,
	[Label] NVARCHAR(50) NOT NULL,
	[Icon] INT NULL,
	[Uid] NVARCHAR(50) NULL,
	[ViewModel] NVARCHAR(50) NULL,
	[ParentID] INT NULL,
	[AppType] INT NOT NULL,
	CONSTRAINT FK_NavigationItem_ParentID FOREIGN KEY ([ParentID]) REFERENCES [Universal].[NavigationItem] ([NavigationItemID])
)
GO
CREATE INDEX IX_NavigationItem_AppType ON [Universal].[NavigationItem] ([AppType]);
