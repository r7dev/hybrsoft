using Hybrsoft.Domain.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.Domain.Dtos
{
	public partial class CountryDto : ObservableObject
	{
		public Int16 CountryID { get; set; }
		public string Name { get; set; }
	}
}
