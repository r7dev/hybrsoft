using Hybrsoft.Domain.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class ClassroomList : UserControl
	{
		public ClassroomList()
		{
			this.InitializeComponent();
		}

		#region ViewModel
		public ClassroomListViewModel ViewModel
		{
			get { return (ClassroomListViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}
		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(ClassroomListViewModel), typeof(ClassroomList), new PropertyMetadata(null));
		#endregion
	}
}
