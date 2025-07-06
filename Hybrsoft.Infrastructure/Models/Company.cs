using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class Company
	{
		public long CompanyID { get; set; }
		public string LegalName { get; set; }
		public string TradeName { get; set; }
		public string FederalRegistration { get; set; }
		public string StateRegistration { get; set; }
		public string CityLicense { get; set; }
		public Int16 CountryID { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{LegalName} {TradeName} {FederalRegistration} {StateRegistration} {CityLicense} {Phone} {Email}".ToLower();

		public virtual Country Country { get; set; }
	}
}
