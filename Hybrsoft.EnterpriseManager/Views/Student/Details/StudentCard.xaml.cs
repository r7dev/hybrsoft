using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class StudentCard : UserControl
	{
		public StudentCard()
		{
			this.InitializeComponent();
		}

		#region ViewModel
		public StudentDetailsViewModel ViewModel
		{
			get { return (StudentDetailsViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(StudentDetailsViewModel), typeof(StudentCard), new PropertyMetadata(null));
		#endregion

		#region Item
		public StudentDto Item
		{
			get { return (StudentDto)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}

		public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(StudentDto), typeof(StudentCard), new PropertyMetadata(null));
		#endregion
	}
}
