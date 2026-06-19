CREATE TABLE [Universal].[AppLogEmbedding]
(
	[AppLogEmbeddingID] BIGINT NOT NULL PRIMARY KEY,
	[Embedding] VECTOR(1536) NULL,
	CONSTRAINT FK_AppLogEmbedding_AppLogID FOREIGN KEY ([AppLogEmbeddingID]) REFERENCES [Universal].[AppLog] ([AppLogID]) ON DELETE CASCADE
)
