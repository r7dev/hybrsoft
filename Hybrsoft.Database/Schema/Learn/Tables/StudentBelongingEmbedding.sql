CREATE TABLE [Learn].[StudentBelongingEmbedding]
(
	[StudentBelongingEmbeddingID] BIGINT NOT NULL PRIMARY KEY,
	[StudentBelongingID] BIGINT NOT NULL,
	[SearchTerms] NVARCHAR(2000) NOT NULL,
	[Embedding] VECTOR(1536) NULL,
	CONSTRAINT FK_StudentBelongingEmbedding_StudentBelongingID FOREIGN KEY ([StudentBelongingID]) REFERENCES [Learn].[StudentBelonging] ([StudentBelongingID]) ON DELETE CASCADE
)
