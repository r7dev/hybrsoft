using System;
using System.ComponentModel.DataAnnotations;

namespace Hybrsoft.Infrastructure.Models
{
	public partial class User
	{
		[Required]
		public long UserId { get; set; }
		[Required]
		[StringLength(50, MinimumLength = 5, ErrorMessage = "The name must be between 5 and 50 characters long")]
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		[Required]
		[StringLength(50, MinimumLength = 5, ErrorMessage = "The lastname must be between 5 and 50 characters long")]
		public string LastName { get; set; }
		[Required]
		[StringLength(150, MinimumLength = 5, ErrorMessage = "The email must be between 5 and 150 characters long")]
		public string Email { get; set; }
		[Required]
		[StringLength(100, MinimumLength = 6, ErrorMessage = "The email must be between 6 and 100 characters long")]
		public string Password { get; set; }
		public int PasswordLength { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{UserId} {FirstName} {LastName} {Email}".ToLower();
	}
}
