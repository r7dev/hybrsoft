using Hybrsoft.UI.Windows.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class DismissibleStudentList : UserControl
	{
		public DismissibleStudentList()
		{
			InitializeComponent();
		}

		#region ViewModel
		public DismissibleStudentListViewModel ViewModel
		{
			get { return (DismissibleStudentListViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}
		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(DismissibleStudentListViewModel), typeof(DismissibleStudentList), new PropertyMetadata(null));
		#endregion
	}
}
