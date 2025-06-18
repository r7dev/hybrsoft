using Hybrsoft.Domain.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.Domain.Dtos
{
	public partial class ScheduleTypeDto : ObservableObject
	{
		public Int16 ScheduleTypeID { get; set; }
		public string Name { get; set; }
	}
}
