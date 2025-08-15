using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.UI.Windows.Dtos
{
	public partial class RoleDto : ObservableObject
	{
		static public RoleDto CreateEmpty() => new() { RoleID = -1, IsEmpty = true };

		public long RoleID { get; set; }

		public string Name { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }

		public bool IsNew => RoleID <= 0;

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
				RoleID = source.RoleID;
				Name = source.Name;
				CreatedOn = source.CreatedOn;
				LastModifiedOn = source.LastModifiedOn;
			}
		}
	}
}
