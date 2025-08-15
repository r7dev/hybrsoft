using Hybrsoft.UI.Windows.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class UsersUserRoles : UserControl
	{
		public UsersUserRoles()
		{
			this.InitializeComponent();
		}

		#region ViewModel
		public UserRoleListViewModel ViewModel
		{
			get { return (UserRoleListViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(UserRoleListViewModel), typeof(UsersUserRoles), new PropertyMetadata(null));
		#endregion
	}
}
