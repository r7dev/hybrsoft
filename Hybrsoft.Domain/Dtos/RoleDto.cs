using Hybrsoft.Domain.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.Domain.Dtos
{
	public partial class RoleDto : ObservableObject
	{
		static public RoleDto CreateEmpty() => new() { RoleId = -1, IsEmpty = true };

		public long RoleId { get; set; }

		public string Name { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }

		public bool IsNew => RoleId <= 0;

		public override void Merge(ObservableObject source)
		{
			if (source is RoleDto model)
			{
				Merge(model);
			}
		}

		public void Merge(RoleDto source)
		{
			if (source != null)
			{
				RoleId = source.RoleId;
				Name = source.Name;
				CreatedOn = source.CreatedOn;
				LastModifiedOn = source.LastModifiedOn;
			}
		}
	}
}
