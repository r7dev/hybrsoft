using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.UI.Windows.Dtos
{
	public partial class CountryDto : ObservableObject
	{
		public Int16 CountryID { get; set; }
		public string Name { get; set; }
	}
}
