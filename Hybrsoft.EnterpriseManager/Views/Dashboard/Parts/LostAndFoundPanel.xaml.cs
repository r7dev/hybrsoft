using Hybrsoft.UI.Windows.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views;

public sealed partial class LostAndFoundPanel : UserControl
{
	public LostAndFoundPanel()
	{
		InitializeComponent();
	}

	#region ItemsSource
	public IList<LostAndFoundModel> ItemsSource
	{
		get { return (IList<LostAndFoundModel>)GetValue(ItemsSourceProperty); }
		set { SetValue(ItemsSourceProperty, value); }
	}

	public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IList<LostAndFoundModel>), typeof(LostAndFoundPanel), new PropertyMetadata(null));
	#endregion
}
