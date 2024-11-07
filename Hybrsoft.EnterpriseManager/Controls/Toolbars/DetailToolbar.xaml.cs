using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Hybrsoft.EnterpriseManager.Tools.ElementSet;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Controls
{
	public sealed partial class DetailToolbar : UserControl
	{
		public event ToolbarButtonClickEventHandler ButtonClick;

		public DetailToolbar()
		{
			this.InitializeComponent();
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			UpdateControl();
		}

		#region ToolbarMode
		public DetailToolbarMode ToolbarMode
		{
			get { return (DetailToolbarMode)GetValue(ToolbarModeProperty); }
			set { SetValue(ToolbarModeProperty, value); }
		}

		private static void ToolbarModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as DetailToolbar;
			control.UpdateControl();
		}

		public static readonly DependencyProperty ToolbarModeProperty = DependencyProperty.Register("ToolbarMode", typeof(DetailToolbarMode), typeof(DetailToolbar), new PropertyMetadata(DetailToolbarMode.Default, ToolbarModeChanged));
		#endregion

		#region DefaultCommands*
		public string DefaultCommands
		{
			get { return (string)GetValue(DefaultCommandsProperty); }
			set { SetValue(DefaultCommandsProperty, value); }
		}

		private static void DefaultCommandsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as DetailToolbar;
			control.UpdateControl();
		}

		public static readonly DependencyProperty DefaultCommandsProperty = DependencyProperty.Register(nameof(DefaultCommands), typeof(string), typeof(DetailToolbar), new PropertyMetadata("edit,delete", DefaultCommandsChanged));
		#endregion

		private void UpdateControl()
		{
			switch (ToolbarMode)
			{
				default:
				case DetailToolbarMode.Default:
					ShowCategory(DefaultCommands.Split(','));
					break;
				case DetailToolbarMode.BackEditdDelete:
					ShowCategory("back", "edit", "delete");
					break;
				case DetailToolbarMode.CancelSave:
					ShowCategory("cancel", "save");
					break;
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			ElementSet.Children<AppBarButton>(commandBar.PrimaryCommands).Click += OnButtonClick;
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			ElementSet.Children<AppBarButton>(commandBar.PrimaryCommands).Click -= OnButtonClick;
		}

		private void ShowCategory(params string[] categories)
		{
			ElementSet.Children<AppBarButton>(commandBar.PrimaryCommands)
				.ForEach(v => v.Show(v.IsCategory(categories)));
		}

		private void OnButtonClick(object sender, RoutedEventArgs e)
		{
			if (e.OriginalSource is AppBarButton button)
			{
				switch (button.Name)
				{
					case "buttonBack":
						RaiseButtonClick(ToolbarButton.Back);
						break;
					case "buttonEdit":
						RaiseButtonClick(ToolbarButton.Edit);
						break;
					case "buttonDelete":
						RaiseButtonClick(ToolbarButton.Delete);
						break;
					case "buttonCancel":
						RaiseButtonClick(ToolbarButton.Cancel);
						break;
					case "buttonSave":
						RaiseButtonClick(ToolbarButton.Save);
						break;
				}
			}
		}

		private void RaiseButtonClick(ToolbarButton toolbarButton)
		{
			ButtonClick?.Invoke(this, new ToolbarButtonClickEventArgs(toolbarButton));
		}
	}
}
