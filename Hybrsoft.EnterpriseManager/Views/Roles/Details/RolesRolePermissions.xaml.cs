using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Hybrsoft.Domain.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class RolesRolePermissions : UserControl
	{
		public RolesRolePermissions()
		{
			this.InitializeComponent();
		}

		#region ViewModel
		public RolePermissionListViewModel ViewModel
		{
			get { return (RolePermissionListViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(RolePermissionListViewModel), typeof(RolesRolePermissions), new PropertyMetadata(null));
		#endregion
	}
}
