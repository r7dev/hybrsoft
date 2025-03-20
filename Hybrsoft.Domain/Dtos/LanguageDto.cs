using Hybrsoft.Domain.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.Domain.Dtos
{
	public partial class LanguageDto : ObservableObject
	{
		public string DisplayName { get; set; }
		public string Tag { get; set; }

		#region Fix object bind SelectedItem in ComboBox
		public override bool Equals(object obj)
		{
			if (obj is LanguageDto other)
			{
				return Tag == other.Tag && DisplayName == other.DisplayName;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(DisplayName, Tag);
		}
		#endregion
	}
}
