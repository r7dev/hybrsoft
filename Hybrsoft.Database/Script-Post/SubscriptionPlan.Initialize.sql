PRINT 'Initializing SubscriptionPlan...'

MERGE INTO [Universal].[SubscriptionPlan] AS Target
USING
(
	VALUES
		(1, 'Monthly', 1, 'SubscriptionPlan_Montly'),
		(2, 'Quarterly', 3,'SubscriptionPlan_Quarterly'),
		(3, 'Yearly', 12, 'SubscriptionPlan_Yearly')
) AS Source ([SubscriptionPlanID], [Name], [DurationMonths], [Uid])
ON Target.[SubscriptionPlanID] = Source.[SubscriptionPlanID]
WHEN NOT MATCHED THEN
	INSERT ([SubscriptionPlanID], [Name], [DurationMonths], [Uid]) 
	VALUES (Source.[SubscriptionPlanID], Source.[Name], Source.[DurationMonths], Source.[Uid])
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;

PRINT 'SubscriptionPlan loaded.'