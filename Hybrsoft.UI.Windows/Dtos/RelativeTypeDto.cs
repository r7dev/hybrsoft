using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.UI.Windows.Dtos
{
	public partial class RelativeTypeDto : ObservableObject
	{
		public Int16 RelativeTypeID { get; set; }
		public string Name { get; set; }
	}
}
