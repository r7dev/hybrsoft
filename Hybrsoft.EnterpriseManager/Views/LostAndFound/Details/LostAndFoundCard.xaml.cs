using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class LostAndFoundCard : UserControl
	{
		public LostAndFoundCard()
		{
			InitializeComponent();
		}

		#region ViewModel
		public LostAndFoundDetailsViewModel ViewModel
		{
			get { return (LostAndFoundDetailsViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(LostAndFoundDetailsViewModel), typeof(LostAndFoundCard), new PropertyMetadata(null));
		#endregion

		#region Item
		public LostAndFoundModel Item
		{
			get { return (LostAndFoundModel)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}

		public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(LostAndFoundModel), typeof(LostAndFoundCard), new PropertyMetadata(null));
		#endregion
	}
}
