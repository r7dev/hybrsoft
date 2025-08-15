using Hybrsoft.UI.Windows.Dtos;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class RolePermissionCard : UserControl
	{
		public RolePermissionCard()
		{
			this.InitializeComponent();
		}

		#region Item
		public RolePermissionDto Item
		{
			get { return (RolePermissionDto)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}

		public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(RolePermissionDto), typeof(RolePermissionCard), new PropertyMetadata(null));
		#endregion
	}
}
