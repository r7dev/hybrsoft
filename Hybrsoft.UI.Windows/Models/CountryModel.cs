using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.UI.Windows.Models
{
	public partial class CountryModel : ObservableObject
	{
		public Int16 CountryID { get; set; }
		public string Name { get; set; }
	}
}
