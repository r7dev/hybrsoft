CREATE TABLE [Learn].[LostAndFoundEmbedding]
(
	[LostAndFoundEmbeddingID] BIGINT NOT NULL PRIMARY KEY,
	[LostAndFoundID] BIGINT NOT NULL,
	[SearchTerms] NVARCHAR(220) NOT NULL,
	[Embedding] VECTOR(1536) NULL,
	CONSTRAINT FK_LostAndFoundEmbedding_LostAndFoundID FOREIGN KEY ([LostAndFoundID]) REFERENCES [Learn].[LostAndFound] ([LostAndFoundID]) ON DELETE CASCADE
)
