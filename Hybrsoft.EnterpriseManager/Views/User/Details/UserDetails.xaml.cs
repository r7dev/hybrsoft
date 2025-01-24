using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Hybrsoft.Domain.ViewModels;
using System.Threading.Tasks;

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
		public UserDetailsWithRolesViewModel ViewModel
		{
			get { return (UserDetailsWithRolesViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(UserDetailsWithRolesViewModel), typeof(UserDetails), new PropertyMetadata(null));
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
