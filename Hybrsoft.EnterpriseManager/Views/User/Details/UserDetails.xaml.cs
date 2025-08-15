using Hybrsoft.UI.Windows.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Linq;
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
			UpdateBindingsAsync();
		}

		private async void UpdateBindingsAsync()
		{
			await Task.Delay(200);
			this.Bindings.Update();
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

		private void FormPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			var control = sender as PasswordBox;
			if (control is not null)
			{
				bool areAllCharEqual = control.Password.All(c => c == ViewModel.PasswordChar);
				ViewModel.UserDetails.EditableItem.PasswordChanged = !areAllCharEqual;
			}
		}
	}
}
