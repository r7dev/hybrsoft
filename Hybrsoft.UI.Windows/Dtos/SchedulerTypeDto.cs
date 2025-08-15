using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.UI.Windows.Dtos
{
	public partial class ScheduleTypeDto : ObservableObject
	{
		public Int16 ScheduleTypeID { get; set; }
		public string Name { get; set; }
	}
}
