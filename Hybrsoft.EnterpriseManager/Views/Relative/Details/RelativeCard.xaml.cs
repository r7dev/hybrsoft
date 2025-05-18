using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class RelativeCard : UserControl
	{
		public RelativeCard()
		{
			InitializeComponent();
		}

		#region ViewModel
		public RelativeDetailsViewModel ViewModel
		{
			get { return (RelativeDetailsViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(RelativeDetailsViewModel), typeof(RelativeCard), new PropertyMetadata(null));
		#endregion

		#region Item
		public RelativeDto Item
		{
			get { return (RelativeDto)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}

		public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(RelativeDto), typeof(RelativeCard), new PropertyMetadata(null));
		#endregion
	}
}
