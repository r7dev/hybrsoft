using Hybrsoft.Domain.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class ClassroomsClassroomStudents : UserControl
	{
		public ClassroomsClassroomStudents()
		{
			this.InitializeComponent();
		}

		#region ViewModel
		public ClassroomStudentListViewModel ViewModel
		{
			get { return (ClassroomStudentListViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(ClassroomStudentListViewModel), typeof(ClassroomsClassroomStudents), new PropertyMetadata(null));
		#endregion
	}
}
