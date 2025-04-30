using Hybrsoft.Domain.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.Domain.Dtos
{
	public partial class PermissionDto : ObservableObject
	{
		static public PermissionDto CreateEmpty() => new() { PermissionID = -1, IsEmpty = true };
		public long PermissionID { get; set; }
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public string Description { get; set; }
		public bool IsEnabled { get; set; }

		public bool IsNew => PermissionID <= 0;
		public string FullName => $"{DisplayName}";

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }

		public override void Merge(ObservableObject source)
		{
			if (source is PermissionDto model)
			{
				Merge(model);
			}
		}

		public void Merge(PermissionDto source)
		{
			if (source != null)
			{
				PermissionID = source.PermissionID;
				Name = source.Name;
				DisplayName = source.DisplayName;
				Description = source.Description;
				IsEnabled = source.IsEnabled;
				CreatedOn = source.CreatedOn;
				LastModifiedOn = source.LastModifiedOn;
			}
		}
	}
}
