﻿using Hybrsoft.Domain.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.Domain.Dtos
{
	public partial class RolePermissionDto : ObservableObject
	{
		public long RolePermissionID { get; set; }

		public long RoleID { get; set; }
		public long PermissionID { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }

		public PermissionDto Permission { get; set; }

		public bool IsNew => RolePermissionID <= 0;

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
				RolePermissionID = source.RolePermissionID;
				RoleID = source.RoleID;
				PermissionID = source.PermissionID;
				Permission = source.Permission;
				CreatedOn = source.CreatedOn;
				LastModifiedOn = source.LastModifiedOn;
			}
		}
	}
}
