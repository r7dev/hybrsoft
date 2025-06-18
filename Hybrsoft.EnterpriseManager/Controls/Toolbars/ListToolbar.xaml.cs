using Hybrsoft.EnterpriseManager.Tools.ElementSet;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Windows.Input;
using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Controls
{
	public sealed partial class ListToolbar : UserControl
	{
		public event ToolbarButtonClickEventHandler ButtonClick;
		public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted;

		public ListToolbar()
		{
			this.InitializeComponent();
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			UpdateControl();
		}

		#region NewLabel
		public string NewLabel
		{
			get { return (string)GetValue(NewLabelProperty); }
			set { SetValue(NewLabelProperty, value); }
		}

		public static readonly DependencyProperty NewLabelProperty = DependencyProperty.Register(nameof(NewLabel), typeof(string), typeof(ListToolbar), new PropertyMetadata("New"));
		#endregion

		#region ToolbarMode
		public ListToolbarMode ToolbarMode
		{
			get { return (ListToolbarMode)GetValue(ToolbarModeProperty); }
			set { SetValue(ToolbarModeProperty, value); }
		}

		private static void ToolbarModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as ListToolbar;
			control.UpdateControl();
		}

		public static readonly DependencyProperty ToolbarModeProperty = DependencyProperty.Register("ToolbarMode", typeof(ListToolbarMode), typeof(ListToolbar), new PropertyMetadata(ListToolbarMode.Default, ToolbarModeChanged));
		#endregion

		#region DefaultCommands*
		public string DefaultCommands
		{
			get { return (string)GetValue(DefaultCommandsProperty); }
			set { SetValue(DefaultCommandsProperty, value); }
		}

		private static void DefaultCommandsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as ListToolbar;
			control.UpdateControl();
		}

		public static readonly DependencyProperty DefaultCommandsProperty = DependencyProperty.Register(nameof(DefaultCommands), typeof(string), typeof(ListToolbar), new PropertyMetadata("new,select,refresh,search", DefaultCommandsChanged));
		#endregion

		#region DefaultCommandsMultipleSelection*
		public string DefaultCommandsMultipleSelection
		{
			get { return (string)GetValue(DefaultCommandsMultipleSelectionProperty); }
			set { SetValue(DefaultCommandsMultipleSelectionProperty, value); }
		}
		public static readonly DependencyProperty DefaultCommandsMultipleSelectionProperty = DependencyProperty.Register(nameof(DefaultCommandsMultipleSelection), typeof(string), typeof(ListToolbar), new PropertyMetadata("cancel,delete", DefaultCommandsChanged));
		#endregion

		#region StartDate
		public DateTimeOffset? StartDate
		{
			get
			{
				var value = GetValue(StartDateProperty);
				return value == null ? (DateTimeOffset?)null : (DateTimeOffset)value;
			}
			set { SetValue(StartDateProperty, value); }
		}

		public static readonly DependencyProperty StartDateProperty = DependencyProperty.Register("StartDate", typeof(DateTimeOffset), typeof(ListToolbar), new PropertyMetadata(null));
		#endregion

		#region EndDate
		public DateTimeOffset? EndDate
		{
			get
			{
				var value = GetValue(EndDateProperty);
				return value == null ? (DateTimeOffset?)null : (DateTimeOffset)value;
			}
			set { SetValue(EndDateProperty, value); }
		}

		public static readonly DependencyProperty EndDateProperty = DependencyProperty.Register("EndDate", typeof(DateTimeOffset), typeof(ListToolbar), new PropertyMetadata(null));
		#endregion

		#region Query
		public string Query
		{
			get { return (string)GetValue(QueryProperty); }
			set { SetValue(QueryProperty, value); }
		}

		public static readonly DependencyProperty QueryProperty = DependencyProperty.Register("Query", typeof(string), typeof(ListToolbar), new PropertyMetadata(null));
		#endregion

		#region NewCommand
		public ICommand NewCommand
		{
			get { return (ICommand)GetValue(NewCommandProperty); }
			set { SetValue(NewCommandProperty, value); }
		}

		public static readonly DependencyProperty NewCommandProperty = DependencyProperty.Register(nameof(NewCommand), typeof(ICommand), typeof(ListToolbar), new PropertyMetadata(null));
		#endregion

		#region DeleteCommand
		public ICommand DeleteCommand
		{
			get { return (ICommand)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}

		public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(ListToolbar), new PropertyMetadata(null));
		#endregion

		#region AcceptCommand
		public ICommand AcceptCommand
		{
			get { return (ICommand)GetValue(AcceptCommandProperty); }
			set { SetValue(AcceptCommandProperty, value); }
		}
		public static readonly DependencyProperty AcceptCommandProperty = DependencyProperty.Register(nameof(AcceptCommand), typeof(ICommand), typeof(ListToolbar), new PropertyMetadata(null));
		#endregion

		private void UpdateControl()
		{
			switch (ToolbarMode)
			{
				default:
				case ListToolbarMode.Default:
					ShowCategory(DefaultCommands.Split(','));
					break;
				case ListToolbarMode.Cancel:
					ShowCategory("cancel");
					break;
				case ListToolbarMode.CancelMore:
					ShowCategory(DefaultCommandsMultipleSelection.Split(','));
					break;
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			ElementSet.Children<AppBarButton>(commandBar.PrimaryCommands).Click += OnButtonClick;
			ElementSet.Children<AppBarButton>(commandBar.Content).Click += OnButtonClick;
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			ElementSet.Children<AppBarButton>(commandBar.PrimaryCommands).Click -= OnButtonClick;
			ElementSet.Children<AppBarButton>(commandBar.Content).Click -= OnButtonClick;
		}

		private void ShowCategory(params string[] categories)
		{
			ElementSet.Children<AppBarButton>(commandBar.PrimaryCommands)
				.ForEach(v => v.Show(v.IsCategory(categories)));
			ElementSet.Children<AppBarButton>(commandBar.Content)
				.ForEach(v => v.Show(v.IsCategory(categories)));
			daterange.Show(daterange.IsCategory(categories));
			search.Show(search.IsCategory(categories));
		}

		private void OnButtonClick(object sender, RoutedEventArgs e)
		{
			if (e.OriginalSource is AppBarButton button)
			{
				switch (button.Name)
				{
					case "buttonNew":
						RaiseButtonClick(ToolbarButton.New);
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
					case "buttonAccept":
						RaiseButtonClick(ToolbarButton.Accept);
						break;
					case "buttonSelect":
						RaiseButtonClick(ToolbarButton.Select);
						break;
					case "buttonRefresh":
						RaiseButtonClick(ToolbarButton.Refresh);
						break;
				}
			}
		}

		private void RaiseButtonClick(ToolbarButton toolbarButton)
		{
			ButtonClick?.Invoke(this, new ToolbarButtonClickEventArgs(toolbarButton));
		}

		private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
		{
			QuerySubmitted?.Invoke(sender, args);
		}
	}
}
