using Hybrsoft.Enums;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace Hybrsoft.EnterpriseManager.Converters
{
	public abstract class EnumToShortConverter<TEnum> : IValueConverter
		where TEnum : struct, Enum
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is TEnum enumValue)
			{
				return (short)(object)enumValue;
			}

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			if (value is short shortValue && Enum.IsDefined(typeof(TEnum), shortValue))
			{
				return (TEnum)(object)shortValue;
			}

			return DependencyProperty.UnsetValue;
		}
	}

	public sealed partial class SubscriptionTypeConverter : EnumToShortConverter<SubscriptionType> { }

	public sealed partial class LostAndFoundStatusConverter : EnumToShortConverter<LostAndFoundStatus> { }
}
