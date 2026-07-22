using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class Student
	{
		public long StudentID { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }

		public byte[] Picture { get; set; }
		public byte[] Thumbnail { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{StudentID} {FirstName} {MiddleName} {LastName} {Email}".ToLower();
	}
}
