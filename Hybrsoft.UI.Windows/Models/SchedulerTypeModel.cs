using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.UI.Windows.Models
{
	public partial class ScheduleTypeModel : ObservableObject
	{
		public Int16 ScheduleTypeID { get; set; }
		public string Name { get; set; }
	}
}
