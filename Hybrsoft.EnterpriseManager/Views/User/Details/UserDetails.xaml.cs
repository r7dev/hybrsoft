using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Hybrsoft.Domain.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class UserDetails : UserControl
	{
		public UserDetails()
		{
			this.InitializeComponent();
		}

		#region ViewModel
		public UserDetailsViewModel ViewModel
		{
			get { return (UserDetailsViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(UserDetailsViewModel), typeof(UserDetails), new PropertyMetadata(null));
		#endregion

		public void SetFocus()
		{
			details.SetFocus();
		}
	}
}
