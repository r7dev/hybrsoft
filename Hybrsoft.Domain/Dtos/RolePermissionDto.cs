using Hybrsoft.Domain.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.Domain.Dtos
{
	public partial class RolePermissionDto : ObservableObject
	{
		public long RolePermissionId { get; set; }

		public long RoleId { get; set; }
		public long PermissionId { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }

		public PermissionDto Permission { get; set; }

		public bool IsNew => RolePermissionId <= 0;

		public override void Merge(ObservableObject source)
		{
			if (source is RolePermissionDto model)
			{
				Merge(model);
			}
		}

		public void Merge(RolePermissionDto source)
		{
			if (source != null)
			{
				RolePermissionId = source.RolePermissionId;
				RoleId = source.RoleId;
				PermissionId = source.PermissionId;
				Permission = source.Permission;
				CreatedOn = source.CreatedOn;
				LastModifiedOn = source.LastModifiedOn;
			}
		}
	}
}
