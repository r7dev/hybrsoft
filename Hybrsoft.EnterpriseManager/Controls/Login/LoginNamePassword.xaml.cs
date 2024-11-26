using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Controls
{
	public sealed partial class LoginNamePassword : UserControl
	{
		public LoginNamePassword()
		{
			this.InitializeComponent();
		}

		#region UserName
		public string UserName
		{
			get { return (string)GetValue(UserNameProperty); }
			set { SetValue(UserNameProperty, value); }
		}

		public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register(nameof(UserName), typeof(string), typeof(LoginNamePassword), new PropertyMetadata(null));
		#endregion

		#region Password
		public string Password
		{
			get { return (string)GetValue(PasswordProperty); }
			set { SetValue(PasswordProperty, value); }
		}

		public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(nameof(Password), typeof(string), typeof(LoginNamePassword), new PropertyMetadata(null));
		#endregion

		#region LoginWithPasswordCommand
		public ICommand LoginWithPasswordCommand
		{
			get { return (ICommand)GetValue(LoginWithPasswordCommandProperty); }
			set { SetValue(LoginWithPasswordCommandProperty, value); }
		}

		public static readonly DependencyProperty LoginWithPasswordCommandProperty = DependencyProperty.Register(nameof(LoginWithPasswordCommand), typeof(ICommand), typeof(LoginNamePassword), new PropertyMetadata(null));
		#endregion

		public void Focus()
		{
			userName.Focus(FocusState.Programmatic);
		}
	}
}
