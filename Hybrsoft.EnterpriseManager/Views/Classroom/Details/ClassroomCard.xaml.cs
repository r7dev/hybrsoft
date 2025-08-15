using Hybrsoft.UI.Windows.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class ClassroomCard : UserControl
	{
		public ClassroomCard()
		{
			this.InitializeComponent();
		}

		#region Item
		public ClassroomModel Item
		{
			get { return (ClassroomModel)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}

		public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(ClassroomModel), typeof(ClassroomCard), new PropertyMetadata(null));
		#endregion
	}
}
