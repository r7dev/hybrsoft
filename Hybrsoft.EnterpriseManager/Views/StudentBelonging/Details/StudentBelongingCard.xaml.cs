using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class StudentBelongingCard : UserControl
	{
		public StudentBelongingCard()
		{
			InitializeComponent();
		}

		#region ViewModel
		public StudentBelongingDetailsViewModel ViewModel
		{
			get { return (StudentBelongingDetailsViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(StudentBelongingDetailsViewModel), typeof(StudentBelongingCard), new PropertyMetadata(null));
		#endregion

		#region Item
		public StudentBelongingModel Item
		{
			get { return (StudentBelongingModel)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}

		public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(StudentBelongingModel), typeof(StudentBelongingCard), new PropertyMetadata(null));
		#endregion
	}
}
