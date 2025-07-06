using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.ViewModels;
using Hybrsoft.EnterpriseManager.Extensions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class CompanyDetails : UserControl
	{
		public CompanyDetails()
		{
			InitializeComponent();
		}

		#region ViewModel
		public CompanyDetailsWithUsersViewModel ViewModel
		{
			get { return (CompanyDetailsWithUsersViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(CompanyDetailsWithUsersViewModel), typeof(CompanyDetails), new PropertyMetadata(null));

		#endregion

		public void SetFocus()
		{
			details.SetFocus();
		}

		public int GetRowSpan(bool isItemNew)
		{
			return isItemNew ? 2 : 1;
		}

		private void FormComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ComboBox comboBox)
			{
				if (comboBox.SelectedItem != null && comboBox.SelectedItem is CountryDto model)
				{
					ViewModel.CompanyDetails.CountrySelectedCommand?.TryExecute(model);
				}
			}
		}
	}
}
