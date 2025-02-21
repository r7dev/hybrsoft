using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class Student
	{
		public long StudentId { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{StudentId} {FirstName} {LastName} {Email}".ToLower();
	}
}
