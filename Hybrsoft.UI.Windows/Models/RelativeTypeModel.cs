using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.UI.Windows.Models
{
	public partial class RelativeTypeModel : ObservableObject
	{
		public Int16 RelativeTypeID { get; set; }
		public string Name { get; set; }
	}
}
