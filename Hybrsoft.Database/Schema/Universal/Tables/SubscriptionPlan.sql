CREATE TABLE [Universal].[SubscriptionPlan]
(
	[SubscriptionPlanID] SMALLINT NOT NULL PRIMARY KEY,
	[Name] NVARCHAR(100) NOT NULL,
	[DurationMonths] SMALLINT NOT NULL,
	[Uid] NVARCHAR(35) NOT NULL,
)
