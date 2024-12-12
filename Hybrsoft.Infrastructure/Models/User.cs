using System;
using System.ComponentModel.DataAnnotations;

namespace Hybrsoft.Infrastructure.Models
{
	public partial class User
	{
		[Required]
		public Guid UserId { get; set; }
		[Required]
		[MaxLength(50)]
		public string FirstName { get; set; }
		[Required]
		[MaxLength(50)]
		public string LastName { get; set; }
		[Required]
		[MaxLength(150)]
		public string Email { get; set; }

		[Required]
		public DateTimeOffset CreatedOn { get; set; }
		[Required]
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{UserId} {FirstName} {LastName}".ToLower();
	}
}
