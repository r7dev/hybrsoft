using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class Relative
	{
		public long RelativeID { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public Int16 RelativeTypeID { get; set; }
		public string DocumentNumber { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }

		public byte[] Picture { get; set; }
		public byte[] Thumbnail { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{RelativeID} {FirstName} {MiddleName} {LastName} {DocumentNumber} {Phone} {Email}".ToLower();

		public virtual RelativeType RelativeType { get; set; }
	}
}
