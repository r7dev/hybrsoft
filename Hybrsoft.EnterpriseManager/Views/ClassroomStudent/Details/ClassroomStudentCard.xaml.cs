using Hybrsoft.UI.Windows.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class ClassroomStudentCard : UserControl
	{
		public ClassroomStudentCard()
		{
			this.InitializeComponent();
		}

		#region Item
		public ClassroomStudentModel Item
		{
			get { return (ClassroomStudentModel)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}

		public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(ClassroomStudentModel), typeof(ClassroomStudentCard), new PropertyMetadata(null));
		#endregion
	}
}
