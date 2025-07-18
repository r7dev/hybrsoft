using Hybrsoft.Domain.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class CompaniesCompanyUsers : UserControl
	{
		public CompaniesCompanyUsers()
		{
			InitializeComponent();
		}

		#region ViewModel
		public CompanyUserListViewModel ViewModel
		{
			get { return (CompanyUserListViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(CompanyUserListViewModel), typeof(CompaniesCompanyUsers), new PropertyMetadata(null));
		#endregion
	}
}
