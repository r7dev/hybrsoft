using System;

namespace Hybrsoft.Infrastructure.Models
{
	public partial class User
	{
		public long UserID { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public int PasswordLength { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{UserID} {FirstName} {LastName} {Email}".ToLower();
	}
}
