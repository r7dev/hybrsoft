using Hybrsoft.Domain.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views;

public sealed partial class CompanyUserDetails : UserControl
{
	public CompanyUserDetails()
	{
		InitializeComponent();
	}

	#region ViewModel
	public CompanyUserDetailsViewModel ViewModel
	{
		get { return (CompanyUserDetailsViewModel)GetValue(ViewModelProperty); }
		set { SetValue(ViewModelProperty, value); }
	}

	public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(CompanyUserDetailsViewModel), typeof(CompanyUserDetails), new PropertyMetadata(null));

	#endregion

	public void SetFocus()
	{
		details.SetFocus();
	}
}
