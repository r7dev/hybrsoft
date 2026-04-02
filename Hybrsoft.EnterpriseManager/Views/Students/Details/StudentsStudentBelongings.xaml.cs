using Hybrsoft.UI.Windows.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class StudentsStudentBelongings : UserControl
	{
		public StudentsStudentBelongings()
		{
			InitializeComponent();
		}

		#region ViewModel
		public StudentBelongingListViewModel ViewModel
		{
			get { return (StudentBelongingListViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(StudentBelongingListViewModel), typeof(StudentsStudentBelongings), new PropertyMetadata(null));
		#endregion
	}
}
