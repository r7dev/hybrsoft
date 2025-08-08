CREATE TABLE [Universal].[Subscription]
(
	[SubscriptionID] BIGINT NOT NULL PRIMARY KEY,
	[SubscriptionPlanID] SMALLINT NOT NULL,
	[DurationDays] SMALLINT NOT NULL,
	[StartDate] DATETIMEOFFSET NULL,
	[ExpirationDate] DATETIMEOFFSET NULL,
	[Type] SMALLINT NOT NULL, -- 0 - Enterprise, 1 - Individual
	[CompanyID] BIGINT NULL,
	[UserID] BIGINT NULL,
	[LicenseKey] NVARCHAR(29) NOT NULL,
	[Status] AS(
		CAST(
			CASE 
				WHEN [StartDate] IS NULL AND [CancelledOn] IS NULL THEN 3 -- Waiting Activation
				WHEN [ExpirationDate] < SYSDATETIMEOFFSET() THEN 2 -- Expired
				WHEN [CancelledOn] IS NOT NULL THEN 1 -- Canceled
				ELSE 0 -- Active
			END
			AS SMALLINT)),
	[CancelledOn] DATETIMEOFFSET NULL,
	[LastValidatedOn] DATETIMEOFFSET NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(200) NULL,
	CONSTRAINT FK_Subscription_SubscriptionPlanID FOREIGN KEY ([SubscriptionPlanID]) REFERENCES [Universal].[SubscriptionPlan] ([SubscriptionPlanID]),
	CONSTRAINT CK_Subscription_DurationDays CHECK ([DurationDays] > 0),
	CONSTRAINT CK_Subscription_Type CHECK ([Type] IN (0, 1) AND 
		(([Type] = 0 AND [CompanyID] IS NOT NULL AND [UserID] IS NULL) OR
		([Type] = 1 AND [CompanyID] IS NULL AND [UserID] IS NOT NULL))),
	CONSTRAINT FK_Subscription_CompanyID FOREIGN KEY ([CompanyID]) REFERENCES [Universal].[Company] ([CompanyID]),
	CONSTRAINT FK_Subscription_UserID FOREIGN KEY ([UserID]) REFERENCES [Universal].[User] ([UserID])
)
GO
CREATE INDEX IX_Subscription_CreatedOn_SearchTerms
ON [Universal].[Subscription] ([CreatedOn], [SearchTerms])
INCLUDE ([LicenseKey], [ExpirationDate], [CancelledOn]);
