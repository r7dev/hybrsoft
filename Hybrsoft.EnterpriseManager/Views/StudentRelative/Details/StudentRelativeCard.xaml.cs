using Hybrsoft.UI.Windows.Dtos;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views;

public sealed partial class StudentRelativeCard : UserControl
{
	public StudentRelativeCard()
	{
		InitializeComponent();
	}

	#region Item
	public StudentRelativeDto Item
	{
		get { return (StudentRelativeDto)GetValue(ItemProperty); }
		set { SetValue(ItemProperty, value); }
	}

	public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(StudentRelativeDto), typeof(StudentRelativeCard), new PropertyMetadata(null));
	#endregion
}
