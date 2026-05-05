using Hybrsoft.Enums;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace Hybrsoft.EnterpriseManager.Converters
{
	public partial class DataLayoutConverter : IValueConverter
	{
		public DataTemplate ListTemplate { get; set; }
		public DataTemplate GridTemplate { get; set; }
		public DataTemplate GridSmallTemplate { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return value switch
			{
				DataLayoutType.List => ListTemplate,
				DataLayoutType.Grid => GridTemplate,
				DataLayoutType.GridSmall => GridSmallTemplate,
				_ => ListTemplate
			};
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
			=> throw new NotImplementedException();
	}
}
