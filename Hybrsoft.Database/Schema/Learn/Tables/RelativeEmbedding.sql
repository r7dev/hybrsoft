CREATE TABLE [Learn].[RelativeEmbedding]
(
	[RelativeEmbeddingID] BIGINT NOT NULL PRIMARY KEY,
	[RelativeID] BIGINT NOT NULL,
	[SearchTerms] NVARCHAR(350) NOT NULL,
	[Embedding] VECTOR(1536) NULL,
	CONSTRAINT FK_RelativeEmbedding_RelativeID FOREIGN KEY ([RelativeID]) REFERENCES [Learn].[Relative] ([RelativeID]) ON DELETE CASCADE
)
