using Hybrsoft.Domain.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class DismissalList : UserControl
	{
		public DismissalList()
		{
			InitializeComponent();
		}

		#region ViewModel
		public DismissalListViewModel ViewModel
		{
			get { return (DismissalListViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}
		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(DismissalListViewModel), typeof(DismissalList), new PropertyMetadata(null));
		#endregion
	}
}
