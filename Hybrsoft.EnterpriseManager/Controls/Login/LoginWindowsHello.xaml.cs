using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Controls
{
	public sealed partial class LoginWindowsHello : UserControl
	{
		public LoginWindowsHello()
		{
			this.InitializeComponent();
		}

		#region UserName
		public string UserName
		{
			get { return (string)GetValue(UserNameProperty); }
			set { SetValue(UserNameProperty, value); }
		}

		public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register(nameof(UserName), typeof(string), typeof(LoginWindowsHello), new PropertyMetadata(null));
		#endregion

		#region LoginWithWindowHelloCommand
		public ICommand LoginWithWindowHelloCommand
		{
			get { return (ICommand)GetValue(LoginWithWindowHelloCommandProperty); }
			set { SetValue(LoginWithWindowHelloCommandProperty, value); }
		}

		public static readonly DependencyProperty LoginWithWindowHelloCommandProperty = DependencyProperty.Register(nameof(LoginWithWindowHelloCommand), typeof(ICommand), typeof(LoginWindowsHello), new PropertyMetadata(null));
		#endregion
	}
}
