﻿using System;

namespace Hybrsoft.Infrastructure.Models
{
	public partial class Category
	{
		public int CategoryId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{CategoryId} {Name} {Description}".ToLower();
	}
}
