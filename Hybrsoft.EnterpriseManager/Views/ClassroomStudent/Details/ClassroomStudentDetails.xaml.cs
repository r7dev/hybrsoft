using Hybrsoft.Domain.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class ClassroomStudentDetails : UserControl
	{
		public ClassroomStudentDetails()
		{
			this.InitializeComponent();
		}

		#region ViewModel
		public ClassroomStudentDetailsViewModel ViewModel
		{
			get { return (ClassroomStudentDetailsViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(ClassroomStudentDetailsViewModel), typeof(ClassroomStudentDetails), new PropertyMetadata(null));

		#endregion

		public void SetFocus()
		{
			details.SetFocus();
		}
	}
}
