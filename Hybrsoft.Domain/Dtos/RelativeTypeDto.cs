using Hybrsoft.Domain.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.Domain.Dtos
{
	public partial class RelativeTypeDto : ObservableObject
	{
		public Int16 RelativeTypeID { get; set; }
		public string Name { get; set; }
		public string LanguageTag { get; set; }
	}
}
