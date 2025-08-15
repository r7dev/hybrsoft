using Hybrsoft.UI.Windows.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class RoleCard : UserControl
	{
		public RoleCard()
		{
			this.InitializeComponent();
		}

		#region Item
		public RoleModel Item
		{
			get { return (RoleModel)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}

		public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(RoleModel), typeof(RoleCard), new PropertyMetadata(null));
		#endregion
	}
}
