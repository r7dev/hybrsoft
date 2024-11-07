using Hybrsoft.Domain.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.Domain.Dtos
{
	public class UserDto : ObservableObject
	{
		static public UserDto CreateEmpty() => new UserDto { UserID = Guid.Empty, IsEmpty = true };
		public Guid UserID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }

		public bool IsNew => UserID == Guid.Empty;
		public string FullName => $"{FirstName} {LastName}";

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
	}
}
