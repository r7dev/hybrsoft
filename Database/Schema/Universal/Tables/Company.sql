CREATE TABLE [Universal].[Company]
(
	[CompanyID] BIGINT NOT NULL PRIMARY KEY,
	[LegalName] NVARCHAR(150) NOT NULL,
	[TradeName] NVARCHAR(150) NULL,
	[FederalRegistration] NVARCHAR(20) NULL,
	[StateRegistration] NVARCHAR(50) NULL,
	[CityLicense] NVARCHAR(50) NULL,
	[CountryID] SMALLINT NOT NULL,
	[Phone] NVARCHAR(50) NULL,
	[Email] NVARCHAR(150) NULL,
	[CreatedOn] DATETIMEOFFSET NOT NULL,
	[LastModifiedOn] DATETIMEOFFSET NULL,
	[SearchTerms] NVARCHAR(400) NULL,
	CONSTRAINT FK_Company_CountryID FOREIGN KEY ([CountryID]) REFERENCES [Universal].[Country] ([CountryID])
)
