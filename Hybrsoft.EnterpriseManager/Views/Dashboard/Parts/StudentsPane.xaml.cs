using Hybrsoft.Domain.Dtos;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class StudentsPane : UserControl
	{
		public StudentsPane()
		{
			InitializeComponent();
		}

		#region ItemsSource
		public IList<StudentDto> ItemsSource
		{
			get { return (IList<StudentDto>)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IList<StudentDto>), typeof(StudentsPane), new PropertyMetadata(null));
		#endregion
	}
}
