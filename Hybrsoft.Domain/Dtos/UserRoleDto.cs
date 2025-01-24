using Hybrsoft.Domain.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.Domain.Dtos
{
	public partial class UserRoleDto : ObservableObject
	{
		public long UserRoleId { get; set; }

		public long UserId { get; set; }
		public long RoleId { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }

		public RoleDto Role { get; set; }

		public bool IsNew => UserRoleId <= 0;

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
				UserRoleId = source.UserRoleId;
				UserId = source.UserId;
				RoleId = source.RoleId;
				Role = source.Role;
				CreatedOn = source.CreatedOn;
				LastModifiedOn = source.LastModifiedOn;
			}
		}
	}
}
