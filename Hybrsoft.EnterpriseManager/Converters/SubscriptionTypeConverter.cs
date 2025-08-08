using Hybrsoft.Enums;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace Hybrsoft.EnterpriseManager.Converters
{
	public sealed partial class SubscriptionTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is SubscriptionType enumValue)
			{
				return (short)enumValue;
			}

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			if (value is short shortValue && Enum.IsDefined(typeof(SubscriptionType), shortValue))
			{
				return (SubscriptionType)shortValue;
			}

			return DependencyProperty.UnsetValue;
		}
	}
}
