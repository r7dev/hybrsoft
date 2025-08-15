using Hybrsoft.UI.Windows.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class DismissalDetails : UserControl
	{
		public DismissalDetails()
		{
			InitializeComponent();
		}

		#region ViewModel
		public DismissalDetailsViewModel ViewModel
		{
			get { return (DismissalDetailsViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(DismissalDetailsViewModel), typeof(DismissalDetails), new PropertyMetadata(null));
		#endregion

		public void SetFocus()
		{
			details.SetFocus();
		}
	}
}
