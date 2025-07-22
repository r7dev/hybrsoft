using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Hybrsoft.EnterpriseManager.Tools.ElementSet;
using System.Windows.Input;

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

		#region BackCommand
		public ICommand BackCommand
		{
			get { return (ICommand)GetValue(BackCommandProperty); }
			set { SetValue(BackCommandProperty, value); }
		}

		public static readonly DependencyProperty BackCommandProperty = DependencyProperty.Register(nameof(BackCommand), typeof(ICommand), typeof(DetailToolbar), new PropertyMetadata(null));
		#endregion

		#region EditCommand
		public ICommand EditCommand
		{
			get { return (ICommand)GetValue(EditCommandProperty); }
			set { SetValue(EditCommandProperty, value); }
		}

		public static readonly DependencyProperty EditCommandProperty = DependencyProperty.Register(nameof(EditCommand), typeof(ICommand), typeof(DetailToolbar), new PropertyMetadata(null));
		#endregion

		#region DeleteCommand
		public ICommand DeleteCommand
		{
			get { return (ICommand)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}

		public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(DetailToolbar), new PropertyMetadata(null));
		#endregion

		#region SaveCommand
		public ICommand SaveCommand
		{
			get { return (ICommand)GetValue(SaveCommandProperty); }
			set { SetValue(SaveCommandProperty, value); }
		}

		public static readonly DependencyProperty SaveCommandProperty = DependencyProperty.Register(nameof(SaveCommand), typeof(ICommand), typeof(DetailToolbar), new PropertyMetadata(null));
		#endregion

		#region CancelCommand
		public ICommand CancelCommand
		{
			get { return (ICommand)GetValue(CancelCommandProperty); }
			set { SetValue(CancelCommandProperty, value); }
		}

		public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register(nameof(CancelCommand), typeof(ICommand), typeof(DetailToolbar), new PropertyMetadata(null));
		#endregion

		#region CancelSecondaryCommand
		public ICommand CancelSecondaryCommand
		{
			get { return (ICommand)GetValue(CancelSecondaryCommandProperty); }
			set { SetValue(CancelSecondaryCommandProperty, value); }
		}

		public static readonly DependencyProperty CancelSecondaryCommandProperty = DependencyProperty.Register(nameof(CancelSecondaryCommand), typeof(ICommand), typeof(DetailToolbar), new PropertyMetadata(null));
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
					case "buttonCancelSecondary":
						RaiseButtonClick(ToolbarButton.CancelSecondary);
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
