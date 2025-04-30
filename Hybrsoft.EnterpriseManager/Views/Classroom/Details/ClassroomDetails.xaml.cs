using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.ViewModels;
using Hybrsoft.EnterpriseManager.Extensions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class ClassroomDetails : UserControl
	{
		public ClassroomDetails()
		{
			this.InitializeComponent();
		}

		#region ViewModel
		public ClassroomDetailsViewModel ViewModel
		{
			get { return (ClassroomDetailsViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(ClassroomDetailsViewModel), typeof(ClassroomDetails), new PropertyMetadata(null));

		#endregion

		public void SetFocus()
		{
			details.SetFocus();
		}

		private void FormComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ComboBox comboBox)
			{
				if (comboBox.SelectedItem != null && comboBox.SelectedItem is ScheduleTypeDto scheduleType)
				{
					ViewModel.ScheduleTypeSelectedCommand?.TryExecute(scheduleType);
				}
			}
		}
	}
}
