using System;
using System.ComponentModel.DataAnnotations;

namespace Hybrsoft.Infrastructure.Models
{
	public partial class Category
	{
		[Key]
		public int CategoryId { get; set; }
		[Required]
		[StringLength(50, MinimumLength = 5, ErrorMessage = "The name must be between 5 and 50 characters long")]
		public string Name { get; set; }
		[Required]
		[StringLength(400, MinimumLength = 5, ErrorMessage = "The description must be between 5 and 400 characters long")]
		public string Description { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{CategoryId} {Name} {Description}".ToLower();
	}
}
