﻿using Microsoft.UI.Xaml.Data;
using System;

namespace Hybrsoft.EnterpriseManager.Converters
{
	public sealed partial class BoolNegationConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return value is bool b && !b;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return value is bool b && !b;
		}
	}
}
