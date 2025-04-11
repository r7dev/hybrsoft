using Hybrsoft.Domain.Dtos;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class UserCard : UserControl
	{
		public UserCard()
		{
			this.InitializeComponent();
		}

		#region Item
		public UserDto Item
		{
			get { return (UserDto)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}

		public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(UserDto), typeof(UserCard), new PropertyMetadata(null));
		#endregion
	}
}
