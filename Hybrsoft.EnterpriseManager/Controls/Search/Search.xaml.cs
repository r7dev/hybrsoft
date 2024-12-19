using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Controls
{
	public sealed partial class Search : UserControl
	{
		public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted;

		public Search()
		{
			this.InitializeComponent();
		}

		#region Query
		public string Query
		{
			get { return (string)GetValue(QueryProperty); }
			set { SetValue(QueryProperty, value); }
		}

		public static readonly DependencyProperty QueryProperty = DependencyProperty.Register("Query", typeof(string), typeof(Search), new PropertyMetadata(null));
		#endregion

		private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
		{
			QuerySubmitted?.Invoke(sender, args);
		}
	}
}
