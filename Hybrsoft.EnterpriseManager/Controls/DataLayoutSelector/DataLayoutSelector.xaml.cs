using Hybrsoft.Enums;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Controls
{
	public sealed partial class DataLayoutSelector : UserControl
	{
		public DataLayoutSelector()
		{
			InitializeComponent();
			Loaded += DataLayoutSelector_Loaded;
		}

		#region Properties
		public DataLayoutType? SelectedDataLayout
		{
			get => (DataLayoutType?)GetValue(SelectedDataLayoutProperty);
			set => SetValue(SelectedDataLayoutProperty, value);
		}

		public static readonly DependencyProperty SelectedDataLayoutProperty =
			DependencyProperty.Register(
				nameof(SelectedDataLayout),
				typeof(DataLayoutType?),
				typeof(DataLayoutSelector),
				new PropertyMetadata(null, OnSelectedDataLayoutChanged));

		private static void OnSelectedDataLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = (DataLayoutSelector)d;
			var layout = (DataLayoutType?)e.NewValue;

			if (layout.HasValue)
			{
				control.UpdateSelectionUI((DataLayoutType)e.NewValue);
			}
		}
		#endregion

		private void DataLayoutSelector_Loaded(object sender, RoutedEventArgs e)
		{
			if (SelectedDataLayout.HasValue)
			{
				UpdateSelectionUI((DataLayoutType)SelectedDataLayout);
			}
		}

		private void SelectorDataLayoutBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
		{
			if (SelectorDataLayoutBar.SelectedItem is SelectorBarItem item &&
				Enum.TryParse<DataLayoutType>(item.Tag?.ToString(), out var result))
			{
				if (SelectedDataLayout != result)
				{
					SelectedDataLayout = result;
				}
			}
		}

		private void UpdateSelectionUI(DataLayoutType layout)
		{
			var desiredItem = layout switch
			{
				DataLayoutType.List => ListItem,
				DataLayoutType.Grid => GridItem,
				DataLayoutType.GridSmall => GridSmallItem,
				_ => null
			};

			if (SelectorDataLayoutBar.SelectedItem != desiredItem)
			{
				SelectorDataLayoutBar.SelectedItem = desiredItem;
			}
		}
	}
}
