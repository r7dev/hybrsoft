using Hybrsoft.EnterpriseManager.Extensions;
using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class LostAndFoundDetails : UserControl
	{
		public LostAndFoundDetails()
		{
			InitializeComponent();
		}

		#region ViewModel
		public LostAndFoundDetailsViewModel ViewModel
		{
			get { return (LostAndFoundDetailsViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(LostAndFoundDetailsViewModel), typeof(LostAndFoundDetails), new PropertyMetadata(null));
		#endregion

		public void SetFocus()
		{
			details.SetFocus();
		}

		private void FormComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ComboBox comboBox)
			{
				if (comboBox.SelectedItem != null && comboBox.SelectedItem is LostAndFoundStatusModel model)
				{
					ViewModel.LostAndFoundStatusSelectedCommand?.TryExecute(model);
				}
			}
		}
	}
}
