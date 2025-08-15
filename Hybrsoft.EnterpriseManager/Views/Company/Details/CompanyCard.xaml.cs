using Hybrsoft.UI.Windows.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	public sealed partial class CompanyCard : UserControl
	{
		public CompanyCard()
		{
			InitializeComponent();
		}

		#region Item
		public CompanyModel Item
		{
			get { return (CompanyModel)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}

		public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(CompanyModel), typeof(CompanyCard), new PropertyMetadata(null));
		#endregion
	}
}
