using Hybrsoft.Infrastructure.Enums;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace Hybrsoft.EnterpriseManager.Converters
{
	public sealed class LogTypeConverter : IValueConverter
	{
		private readonly SolidColorBrush InformationColor = new(Colors.Navy);
		private readonly SolidColorBrush SuccessColor = new(Colors.Green);
		private readonly SolidColorBrush WarningColor = new(Colors.Gold);
		private readonly SolidColorBrush ErrorColor = new(Colors.IndianRed);

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (targetType == typeof(String))
			{
				if (value is LogType logType)
				{
					switch (logType)
					{
						case LogType.Information:
							return Char.ConvertFromUtf32(0xE946).ToString();
						case LogType.Success:
							return Char.ConvertFromUtf32(0xEC61).ToString();
						case LogType.Warning:
							return Char.ConvertFromUtf32(0xE814).ToString();
						case LogType.Error:
							return Char.ConvertFromUtf32(0xEB90).ToString();
					}
				}
				return Char.ConvertFromUtf32(0xE946).ToString();
			}

			if (targetType == typeof(Brush))
			{
				if (value is LogType logType)
				{
					switch (logType)
					{
						case LogType.Information:
							return InformationColor;
						case LogType.Success:
							return SuccessColor;
						case LogType.Warning:
							return WarningColor;
						case LogType.Error:
							return ErrorColor;
					}
				}
				return InformationColor;
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
