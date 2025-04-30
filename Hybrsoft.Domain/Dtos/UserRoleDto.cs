using Hybrsoft.Domain.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.Domain.Dtos
{
	public partial class UserRoleDto : ObservableObject
	{
		public long UserRoleID { get; set; }

		public long UserID { get; set; }
		public long RoleID { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }

		public RoleDto Role { get; set; }

		public bool IsNew => UserRoleID <= 0;

		public override void Merge(ObservableObject source)
		{
			if (source is UserRoleDto model)
			{
				Merge(model);
			}
		}

		public void Merge(UserRoleDto source)
		{
			if (source != null)
			{
				UserRoleID = source.UserRoleID;
				UserID = source.UserID;
				RoleID = source.RoleID;
				Role = source.Role;
				CreatedOn = source.CreatedOn;
				LastModifiedOn = source.LastModifiedOn;
			}
		}
	}
}
