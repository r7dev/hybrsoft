using Hybrsoft.Enums;
using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class LostAndFound
	{
		public long LostAndFoundID { get; set; }
		public string DisplayName { get; set; }
		public string Description { get; set; }

		public LostAndFoundStatus Status { get; set; }
		public long? StudentBelongingID { get; set; }
		public DateTimeOffset? DonationDate { get; set; }

		public byte[] Picture { get; set; }
		public byte[] Thumbnail { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{LostAndFoundID} {DisplayName} {Description}".ToLower();
	}
}
