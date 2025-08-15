using Hybrsoft.UI.Windows.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class PermissionDetails : UserControl
	{
		public PermissionDetails()
		{
			this.InitializeComponent();
		}

		#region ViewModel
		public PermissionDetailsViewModel ViewModel
		{
			get { return (PermissionDetailsViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(PermissionDetailsViewModel), typeof(PermissionDetails), new PropertyMetadata(null));

		#endregion

		public void SetFocus()
		{
			details.SetFocus();
		}
	}
}
