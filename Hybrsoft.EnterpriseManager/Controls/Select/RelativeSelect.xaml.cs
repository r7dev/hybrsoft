using Hybrsoft.Domain.Dtos;
using Hybrsoft.EnterpriseManager.Extensions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections;
using System.Windows.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Controls;

public sealed partial class RelativeSelect : UserControl
{
	public RelativeSelect()
	{
		InitializeComponent();
	}

	#region RelativeSelectedCommand
	public ICommand RelativeSelectedCommand
	{
		get { return (ICommand)GetValue(RelativeSelectedCommandProperty); }
		set { SetValue(RelativeSelectedCommandProperty, value); }
	}

	public static readonly DependencyProperty RelativeSelectedCommandProperty = DependencyProperty.Register(nameof(RelativeSelectedCommand), typeof(ICommand), typeof(RelativeSelect), new PropertyMetadata(null));
	#endregion

	#region ItemsSource
	public IEnumerable ItemsSource
	{
		get => (IEnumerable)GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}
	public static readonly DependencyProperty ItemsSourceProperty =
		DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(RelativeSelect), new PropertyMetadata(null));
	#endregion

	private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (e.AddedItems?.Count > 0)
		{
			var selectedItem = e.AddedItems[0];
			if (selectedItem is RelativeDto relative)
			{
				RelativeSelectedCommand?.TryExecute(relative);
			}
		}
	}
}
