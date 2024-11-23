﻿using Hybrsoft.Infrastructure.Enums;

namespace Hybrsoft.Infrastructure.Models
{
	public class NavigationItem
	{
		public int NavigationItemId { get; set; }
		public string Label { get; set; }
		public int? Icon { get; set; }
		public string Tag { get; set; }
		public int? ParentId { get; set; }
		public AppType AppType { get; set; }
	}
}