using Hybrsoft.Domain.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.Domain.Dtos
{
	public partial class CompanyDto : ObservableObject
	{
		static public CompanyDto CreateEmpty() => new() { CompanyID = -1, IsEmpty = true };
		public long CompanyID { get; set; }
		public string LegalName { get; set; }
		public string TradeName { get; set; }
		public string FederalRegistration { get; set; }
		public string StateRegistration { get; set; }
		public string CityLicense { get; set; }
		public Int16 CountryID { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }

		public bool IsNew => CompanyID <= 0;
		public string FullName => $"{LegalName} ({TradeName})";

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }

		public CountryDto Country { get; set; }

		public override void Merge(ObservableObject source)
		{
			if (source is CompanyDto model)
			{
				Merge(model);
			}
		}

		public void Merge(CompanyDto source)
		{
			if (source != null)
			{
				CompanyID = source.CompanyID;
				LegalName = source.LegalName;
				TradeName = source.TradeName;
				FederalRegistration = source.FederalRegistration;
				StateRegistration = source.StateRegistration;
				CityLicense = source.CityLicense;
				CountryID = source.CountryID;
				Phone = source.Phone;
				Email = source.Email;
				CreatedOn = source.CreatedOn;
				LastModifiedOn = source.LastModifiedOn;
			}
		}
	}
}
