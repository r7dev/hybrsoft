﻿CREATE TABLE [Universal].[NavigationItem]
(
	[NavigationItemId] INT NOT NULL PRIMARY KEY IDENTITY,
	[Label] VARCHAR(50) NOT NULL,
	[Icon] INT NULL,
	[Tag] VARCHAR(50) NOT NULL,
	[ParentId] INT NULL,
	[AppType] INT NOT NULL,
	CONSTRAINT FK_NavigationItem_ParentId FOREIGN KEY ([ParentId]) REFERENCES [Universal].[NavigationItem] ([NavigationItemId]),
	CONSTRAINT UQ_NavigationItem_Tag_AppType UNIQUE ([Tag], [AppType])
)
GO
CREATE INDEX IX_NavigationItem_AppType ON [Universal].[NavigationItem] (AppType);
