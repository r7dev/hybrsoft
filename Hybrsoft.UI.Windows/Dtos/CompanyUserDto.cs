using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.UI.Windows.Dtos
{
	public partial class CompanyUserDto : ObservableObject
	{
		public long CompanyUserID { get; set; }

		public long CompanyID { get; set; }
		public long UserID { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }

		public CompanyDto Company { get; set; }
		public UserDto User { get; set; }

		public bool IsNew => CompanyUserID <= 0;

		public override void Merge(ObservableObject source)
		{
			if (source is CompanyUserDto model)
			{
				Merge(model);
			}
		}

		public void Merge(CompanyUserDto source)
		{
			if (source != null)
			{
				CompanyUserID = source.CompanyUserID;
				CompanyID = source.CompanyID;
				UserID = source.UserID;
				User = source.User;
				CreatedOn = source.CreatedOn;
				LastModifiedOn = source.LastModifiedOn;
			}
		}
	}
}
