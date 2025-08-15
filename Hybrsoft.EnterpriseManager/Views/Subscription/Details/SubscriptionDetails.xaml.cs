using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.ViewModels;
using Hybrsoft.EnterpriseManager.Extensions;
using Hybrsoft.Infrastructure.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class SubscriptionDetails : UserControl
	{
		public SubscriptionDetails()
		{
			InitializeComponent();
		}

		#region ViewModel
		public SubscriptionDetailsViewModel ViewModel
		{
			get { return (SubscriptionDetailsViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(SubscriptionDetailsViewModel), typeof(SubscriptionDetails), new PropertyMetadata(null));

		#endregion

		public void SetFocus()
		{
			details.SetFocus();
		}

		private void FormComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ComboBox comboBox)
			{
				if (comboBox.SelectedItem != null)
				{
					if (comboBox.SelectedItem is SubscriptionPlanModel model)
					{
						ViewModel.SubscriptionPlanSelectedCommand?.TryExecute(model);
					}
					else if (comboBox.SelectedItem is SubscriptionTypeModel modelType)
					{
						ViewModel.SubscriptionTypeSelectedCommand?.TryExecute(modelType);
					}
				}
			}
		}
	}
}
