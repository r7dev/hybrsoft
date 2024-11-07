using System;

namespace Hybrsoft.Domain.Infrastructure.Commom
{
	public class UserInfo
	{
		static readonly public UserInfo Default = new UserInfo
		{
			AccountName = "Lacey Heath",
			FirstName = "Lacey",
			LastName = "Heath"
		};

		public string AccountName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public object PictureSource { get; set; }

		public string DisplayName => $"{FirstName} {LastName}";

		public bool IsEmpty => String.IsNullOrEmpty(DisplayName.Trim());
	}
}
