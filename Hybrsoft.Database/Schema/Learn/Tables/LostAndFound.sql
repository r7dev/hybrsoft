CREATE TABLE [Learn].[LostAndFound]
(
	[LostAndFoundID] BIGINT NOT NULL PRIMARY KEY,
	[DisplayName] NVARCHAR(50) NOT NULL,
	[Description] NVARCHAR(150) NOT NULL,
	[Picture] VARBINARY(MAX) NOT NULL,
	[Thumbnail] VARBINARY(MAX) NOT NULL,
	[Status] SMALLINT NOT NULL,
	[StudentBelongingID] BIGINT NULL,
	[DonationDate] DATETIMEOFFSET NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(220) NULL,
	CONSTRAINT CK_LostAndFound_Status CHECK ([Status] IN (0, 1, 2)), -- 0 - Pending, 1 - Claimed, 2 - Donated
	CONSTRAINT CK_LostAndFound_ClaimedRequiresBelonging CHECK (NOT ([Status] = 1 AND [StudentBelongingID] IS NULL)),
	CONSTRAINT FK_LostAndFound_StudentBelongingID FOREIGN KEY ([StudentBelongingID]) REFERENCES [Learn].[StudentBelonging] ([StudentBelongingID]) ON DELETE NO ACTION
)
GO
CREATE INDEX IX_LostAndFound_DisplayName_SearchTerms
ON [Learn].[LostAndFound] ([DisplayName], [SearchTerms])
INCLUDE ([LostAndFoundID], [Status], [Thumbnail], [LastModifiedOn]);
