using Hybrsoft.UI.Windows.Infrastructure.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Controls
{
	public sealed partial class SearchDateRange : UserControl
	{
		public SearchDateRange()
		{
			this.InitializeComponent();
		}

		#region StartDate
		public DateTimeOffset? StartDate
		{
			get
			{
				var value = GetValue(StartDateProperty);
				return value == null ? (DateTimeOffset?)null : (DateTimeOffset)value;
			}
			set { SetValue(StartDateProperty, value); }
		}

		public static readonly DependencyProperty StartDateProperty = DependencyProperty.Register("StartDate", typeof(DateTimeOffset), typeof(SearchDateRange), new PropertyMetadata(null));
		#endregion

		#region EndDate
		public DateTimeOffset? EndDate
		{
			get
			{
				var value = GetValue(EndDateProperty);
				return value == null ? (DateTimeOffset?)null : (DateTimeOffset)value;
			}
			set { SetValue(EndDateProperty, value); }
		}

		public static readonly DependencyProperty EndDateProperty = DependencyProperty.Register("EndDate", typeof(DateTimeOffset), typeof(SearchDateRange), new PropertyMetadata(null));
		#endregion

		private void StartDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
		{
			if (sender.Date.HasValue)
			{
				StartDatePicker.Date = sender.Date.Value.Date;
			}
		}

		private void EndDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
		{
			if (sender.Date.HasValue)
			{
				EndDatePicker.Date = DateRangeTools.GetEndDate(sender.Date.Value);
			}
		}
	}
}
