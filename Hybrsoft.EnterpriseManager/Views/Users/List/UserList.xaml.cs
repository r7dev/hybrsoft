using Hybrsoft.Domain.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class UserList : UserControl
	{
		public UserList()
		{
			this.InitializeComponent();
		}

		#region ViewModel
		public UserListViewModel ViewModel
		{
			get { return (UserListViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}
		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(UserListViewModel), typeof(UserList), new PropertyMetadata(null));
		#endregion
	}
}
