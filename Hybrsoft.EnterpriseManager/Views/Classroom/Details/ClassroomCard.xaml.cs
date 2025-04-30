using Hybrsoft.Domain.Dtos;
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
		public ClassroomDto Item
		{
			get { return (ClassroomDto)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}

		public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(ClassroomDto), typeof(ClassroomCard), new PropertyMetadata(null));
		#endregion
	}
}
