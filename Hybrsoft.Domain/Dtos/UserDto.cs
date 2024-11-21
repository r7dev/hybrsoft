using Hybrsoft.Domain.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.Domain.Dtos
{
	public partial class UserDto : ObservableObject
	{
		static public UserDto CreateEmpty() => new() { UserID = Guid.Empty, IsEmpty = true };
		public Guid UserID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }

		public bool IsNew => UserID == Guid.Empty;
		public string FullName => $"{FirstName} {LastName}";

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }

		public override void Merge(ObservableObject source)
		{
			if (source is UserDto model)
			{
				Merge(model);
			}
		}

		public void Merge(UserDto source)
		{
			if (source != null)
			{
				UserID = source.UserID;
				FirstName = source.FirstName;
				LastName = source.LastName;
				Email = source.Email;
				CreatedOn = source.CreatedOn;
				LastModifiedOn = source.LastModifiedOn;
			}
		}
	}
}
