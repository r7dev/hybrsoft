using Hybrsoft.Infrastructure.Enums;

namespace Hybrsoft.Infrastructure.Models
{
	public class NavigationItem
	{
		public int NavigationItemID { get; set; }
		public string Label { get; set; }
		public int? Icon { get; set; }
		public string Uid { get; set; }
		public string ViewModel { get; set; }
		public int? ParentID { get; set; }
		public AppType AppType { get; set; }
	}
}
