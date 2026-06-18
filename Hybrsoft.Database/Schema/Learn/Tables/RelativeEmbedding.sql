CREATE TABLE [Learn].[RelativeEmbedding]
(
	[RelativeEmbeddingID] BIGINT NOT NULL PRIMARY KEY,
	[SearchTerms] NVARCHAR(350) NOT NULL,
	[Embedding] VECTOR(1536) NULL,
	CONSTRAINT FK_RelativeEmbedding_RelativeID FOREIGN KEY ([RelativeEmbeddingID]) REFERENCES [Learn].[Relative] ([RelativeID]) ON DELETE CASCADE
)
