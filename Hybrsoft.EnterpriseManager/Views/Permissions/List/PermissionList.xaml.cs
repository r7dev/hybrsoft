using Hybrsoft.UI.Windows.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class PermissionList : UserControl
	{
		public PermissionList()
		{
			this.InitializeComponent();
		}

		#region ViewModel
		public PermissionListViewModel ViewModel
		{
			get { return (PermissionListViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}
		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(PermissionListViewModel), typeof(PermissionList), new PropertyMetadata(null));
		#endregion
	}
}
