using Hybrsoft.UI.Windows.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class StudentDetails : UserControl
	{
		public StudentDetails()
		{
			this.InitializeComponent();
		}

		#region ViewModel
		public StudentMasterDetailsViewModel ViewModel
		{
			get { return (StudentMasterDetailsViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(StudentMasterDetailsViewModel), typeof(StudentDetails), new PropertyMetadata(null));
		#endregion

		public void SetFocus()
		{
			details.SetFocus();
		}
	}
}
