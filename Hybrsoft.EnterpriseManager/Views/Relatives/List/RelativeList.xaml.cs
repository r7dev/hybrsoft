using Hybrsoft.Domain.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views;

public sealed partial class RelativeList : UserControl
{
	public RelativeList()
	{
		InitializeComponent();
	}

	#region ViewModel
	public RelativeListViewModel ViewModel
	{
		get { return (RelativeListViewModel)GetValue(ViewModelProperty); }
		set { SetValue(ViewModelProperty, value); }
	}
	public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(RelativeListViewModel), typeof(RelativeList), new PropertyMetadata(null));
	#endregion
}
