using Hybrsoft.Domain.Dtos;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class UserRoleCard : UserControl
	{
		public UserRoleCard()
		{
			this.InitializeComponent();
		}

		#region Item
		public UserRoleDto Item
		{
			get { return (UserRoleDto)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}

		public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(UserRoleDto), typeof(UserRoleCard), new PropertyMetadata(null));
		#endregion
	}
}
