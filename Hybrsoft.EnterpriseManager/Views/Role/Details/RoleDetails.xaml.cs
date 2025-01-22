using Hybrsoft.Domain.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class RoleDetails : UserControl
	{
		public RoleDetails()
		{
			this.InitializeComponent();
		}

		#region ViewModel
		public RoleDetailsWithPermissionsViewModel ViewModel
		{
			get { return (RoleDetailsWithPermissionsViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(RoleDetailsWithPermissionsViewModel), typeof(RoleDetails), new PropertyMetadata(null));

		#endregion

		public void SetFocus()
		{
			details.SetFocus();
		}

		public int GetRowSpan(bool isItemNew)
		{
			return isItemNew ? 2 : 1;
		}
	}
}
