using Hybrsoft.UI.Windows.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views;

public sealed partial class SubscriptionList : UserControl
{
	public SubscriptionList()
	{
		InitializeComponent();
	}

	#region ViewModel
	public SubscriptionListViewModel ViewModel
	{
		get { return (SubscriptionListViewModel)GetValue(ViewModelProperty); }
		set { SetValue(ViewModelProperty, value); }
	}
	public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(SubscriptionListViewModel), typeof(SubscriptionList), new PropertyMetadata(null));
	#endregion
}
