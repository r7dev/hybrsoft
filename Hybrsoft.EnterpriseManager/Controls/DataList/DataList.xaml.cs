using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.EnterpriseManager.Extensions;
using Hybrsoft.EnterpriseManager.Tools.DependencyExpressions;
using Hybrsoft.Infrastructure.Enums;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Controls
{
	public sealed partial class DataList : UserControl, INotifyExpressionChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public DataList()
		{
			this.InitializeComponent();
			DependencyExpressions.Initialize(this);
			ResourceService = ServiceLocator.Current.GetService<IResourceService>();
		}

		static private readonly DependencyExpressions DependencyExpressions = new();
		public IResourceService ResourceService { get; }

		#region NewLabel
		public string NewLabel
		{
			get { return (string)GetValue(NewLabelProperty); }
			set { SetValue(NewLabelProperty, value); }
		}

		public static readonly DependencyProperty NewLabelProperty = DependencyProperty.Register(nameof(NewLabel), typeof(string), typeof(DataList), new PropertyMetadata("New"));
		#endregion

		#region ItemsSource*
		public IEnumerable ItemsSource
		{
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as DataList;
			control.UpdateItemsSource(e.NewValue, e.OldValue);
			DependencyExpressions.UpdateDependencies(control, nameof(ItemsSource));
		}

		private void UpdateItemsSource(object newValue, object oldValue)
		{
			if (oldValue is INotifyCollectionChanged oldSource)
			{
				oldSource.CollectionChanged -= OnCollectionChanged;
			}
			if (newValue is INotifyCollectionChanged newSource)
			{
				newSource.CollectionChanged += OnCollectionChanged;
			}
		}

		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(DataList), new PropertyMetadata(null, ItemsSourceChanged));
		#endregion

		#region HeaderTemplate
		public DataTemplate HeaderTemplate
		{
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}

		public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(DataList), new PropertyMetadata(null));
		#endregion

		#region ItemTemplate
		public DataTemplate ItemTemplate
		{
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}

		public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(DataList), new PropertyMetadata(null));
		#endregion

		#region ItemSecondaryActionInvokedCommand
		public ICommand ItemSecondaryActionInvokedCommand
		{
			get { return (ICommand)GetValue(ItemSecondaryActionInvokedCommandProperty); }
			set { SetValue(ItemSecondaryActionInvokedCommandProperty, value); }
		}

		public static readonly DependencyProperty ItemSecondaryActionInvokedCommandProperty = DependencyProperty.Register(nameof(ItemSecondaryActionInvokedCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));
		#endregion

		#region DefaultCommands
		public string DefaultCommands
		{
			get { return (string)GetValue(DefaultCommandsProperty); }
			set { SetValue(DefaultCommandsProperty, value); }
		}

		public static readonly DependencyProperty DefaultCommandsProperty = DependencyProperty.Register(nameof(DefaultCommands), typeof(string), typeof(DataList), new PropertyMetadata("new,select,refresh,search"));
		#endregion

		#region SelectedItem
		public object SelectedItem
		{
			get { return (object)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(DataList), new PropertyMetadata(null));
		#endregion

		#region IsMultipleSelection*
		public bool IsMultipleSelection
		{
			get { return (bool)GetValue(IsMultipleSelectionProperty); }
			set { SetValue(IsMultipleSelectionProperty, value); }
		}

		private static void IsMultipleSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as DataList;
			DependencyExpressions.UpdateDependencies(control, nameof(IsMultipleSelection));
		}

		public static readonly DependencyProperty IsMultipleSelectionProperty = DependencyProperty.Register(nameof(IsMultipleSelection), typeof(bool), typeof(DataList), new PropertyMetadata(null, IsMultipleSelectionChanged));
		#endregion

		#region SelectedItemsCount*
		public int SelectedItemsCount
		{
			get { return (int)GetValue(SelectedItemsCountProperty); }
			set { SetValue(SelectedItemsCountProperty, value); }
		}

		private static void SelectedItemsCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as DataList;
			DependencyExpressions.UpdateDependencies(control, nameof(SelectedItemsCount));
		}

		public static readonly DependencyProperty SelectedItemsCountProperty = DependencyProperty.Register(nameof(SelectedItemsCount), typeof(int), typeof(DataList), new PropertyMetadata(null, SelectedItemsCountChanged));
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

		public static readonly DependencyProperty StartDateProperty = DependencyProperty.Register("StartDate", typeof(DateTimeOffset), typeof(DataList), new PropertyMetadata(null));
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

		public static readonly DependencyProperty EndDateProperty = DependencyProperty.Register("EndDate", typeof(DateTimeOffset), typeof(DataList), new PropertyMetadata(null));
		#endregion

		#region Query
		public string Query
		{
			get { return (string)GetValue(QueryProperty); }
			set { SetValue(QueryProperty, value); }
		}

		public static readonly DependencyProperty QueryProperty = DependencyProperty.Register(nameof(Query), typeof(string), typeof(DataList), new PropertyMetadata(null));
		#endregion

		#region ItemsCount
		public int ItemsCount
		{
			get { return (int)GetValue(ItemsCountProperty); }
			set { SetValue(ItemsCountProperty, value); }
		}

		public static readonly DependencyProperty ItemsCountProperty = DependencyProperty.Register(nameof(ItemsCount), typeof(int), typeof(DataList), new PropertyMetadata(0));
		#endregion


		#region RefreshCommand
		public ICommand RefreshCommand
		{
			get { return (ICommand)GetValue(RefreshCommandProperty); }
			set { SetValue(RefreshCommandProperty, value); }
		}

		public static readonly DependencyProperty RefreshCommandProperty = DependencyProperty.Register(nameof(RefreshCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));
		#endregion

		#region QuerySubmittedCommand
		public ICommand QuerySubmittedCommand
		{
			get { return (ICommand)GetValue(QuerySubmittedCommandProperty); }
			set { SetValue(QuerySubmittedCommandProperty, value); }
		}

		public static readonly DependencyProperty QuerySubmittedCommandProperty = DependencyProperty.Register(nameof(QuerySubmittedCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));
		#endregion

		#region NewCommand
		public ICommand NewCommand
		{
			get { return (ICommand)GetValue(NewCommandProperty); }
			set { SetValue(NewCommandProperty, value); }
		}

		public static readonly DependencyProperty NewCommandProperty = DependencyProperty.Register(nameof(NewCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));
		#endregion

		#region DeleteCommand
		public ICommand DeleteCommand
		{
			get { return (ICommand)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}

		public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));
		#endregion


		#region StartSelectionCommand
		public ICommand StartSelectionCommand
		{
			get { return (ICommand)GetValue(StartSelectionCommandProperty); }
			set { SetValue(StartSelectionCommandProperty, value); }
		}

		public static readonly DependencyProperty StartSelectionCommandProperty = DependencyProperty.Register(nameof(StartSelectionCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));
		#endregion

		#region CancelSelectionCommand
		public ICommand CancelSelectionCommand
		{
			get { return (ICommand)GetValue(CancelSelectionCommandProperty); }
			set { SetValue(CancelSelectionCommandProperty, value); }
		}

		public static readonly DependencyProperty CancelSelectionCommandProperty = DependencyProperty.Register(nameof(CancelSelectionCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));
		#endregion

		#region SelectItemsCommand
		public ICommand SelectItemsCommand
		{
			get { return (ICommand)GetValue(SelectItemsCommandProperty); }
			set { SetValue(SelectItemsCommandProperty, value); }
		}

		public static readonly DependencyProperty SelectItemsCommandProperty = DependencyProperty.Register(nameof(SelectItemsCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));
		#endregion

		#region DeselectItemsCommand
		public ICommand DeselectItemsCommand
		{
			get { return (ICommand)GetValue(DeselectItemsCommandProperty); }
			set { SetValue(DeselectItemsCommandProperty, value); }
		}

		public static readonly DependencyProperty DeselectItemsCommandProperty = DependencyProperty.Register(nameof(DeselectItemsCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));
		#endregion

		#region SelectRangesCommand
		public ICommand SelectRangesCommand
		{
			get { return (ICommand)GetValue(SelectRangesCommandProperty); }
			set { SetValue(SelectRangesCommandProperty, value); }
		}

		public static readonly DependencyProperty SelectRangesCommandProperty = DependencyProperty.Register(nameof(SelectRangesCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));
		#endregion


		public ListToolbarMode ToolbarMode => IsMultipleSelection ? (SelectedItemsCount > 0 ? ListToolbarMode.CancelMore : ListToolbarMode.Cancel) : ListToolbarMode.Default;
		public static readonly DependencyExpression ToolbarModeExpression = DependencyExpressions.Register(
			nameof(ToolbarMode),
			nameof(IsMultipleSelection),
			nameof(SelectedItemsCount)
		);

		public ListViewSelectionMode SelectionMode => IsMultipleSelection ? ListViewSelectionMode.Multiple : ListViewSelectionMode.Single;
		public static readonly DependencyExpression SelectionModeExpression = DependencyExpressions.Register(
			nameof(SelectionMode),
			nameof(IsMultipleSelection)
		);

		public bool IsSingleSelection => !IsMultipleSelection;
		public static readonly DependencyExpression IsSingleSelectionExpression = DependencyExpressions.Register(
			nameof(IsSingleSelection),
			nameof(IsMultipleSelection)
		);

		public bool IsDataAvailable => (ItemsSource?.Cast<object>().Any() ?? false);
		public static readonly DependencyExpression IsDataAvailableExpression = DependencyExpressions.Register(
			nameof(IsDataAvailable),
			nameof(ItemsSource)
		);

		public bool IsDataUnavailable => !IsDataAvailable;
		public static readonly DependencyExpression IsDataUnavailableExpression = DependencyExpressions.Register(
			nameof(IsDataUnavailable),
			nameof(IsDataAvailable)
		);

		public string DataUnavailableMessage
		{
			get
			{
				if (ItemsSource == null)
				{
					return ResourceService.GetString(nameof(ResourceFiles.UI), "DataListUserControl_Loading");
				}
				else
				{
					string searchMessage = ResourceService.GetString(nameof(ResourceFiles.UI), "DataListUserControl_NoItemsFound");
					if (DefaultCommands.Contains("new"))
					{
						string newItemMessage = ResourceService.GetString(nameof(ResourceFiles.UI), "DataListUserControl_NewItem");
						return $"{searchMessage} {newItemMessage}";
					}
					return searchMessage;
				}
			}
		}
		public static readonly DependencyExpression DataUnavailableMessageExpression = DependencyExpressions.Register(
			nameof(DataUnavailableMessage),
			nameof(ItemsSource)
		);

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (!IsMultipleSelection)
			{
				if (ItemsSource is IList list)
				{
					if (e.Action == NotifyCollectionChangedAction.Replace)
					{
						if (ItemsSource is ISelectionInfo selectionInfo)
						{
							if (selectionInfo.IsSelected(e.NewStartingIndex))
							{
								SelectedItem = list[e.NewStartingIndex];
								System.Diagnostics.Debug.WriteLine("SelectedItem {0}", SelectedItem);
							}
						}
					}
				}
			}
		}

		private void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
		{
			if (!IsMultipleSelection)
			{
				ItemSecondaryActionInvokedCommand?.TryExecute(listview.SelectedItem);
			}
		}

		private void OnPreviewKeyDown(object sender, KeyRoutedEventArgs e)
		{
			if (!IsMultipleSelection)
			{
				if (e.Key == Windows.System.VirtualKey.Enter)
				{
					ItemSecondaryActionInvokedCommand?.TryExecute(listview.SelectedItem);
				}
			}
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (IsMultipleSelection)
			{
				if (listview.SelectedItems != null)
				{
					SelectedItemsCount = listview.SelectedItems.Count;
				}
				else if (listview.SelectedRanges != null)
				{
					var ranges = listview.SelectedRanges;
					SelectedItemsCount = ranges.IndexCount();
					SelectRangesCommand?.TryExecute(ranges.GetIndexRanges().ToArray());
				}

				if (e.AddedItems != null)
				{
					SelectItemsCommand?.TryExecute(e.AddedItems);
				}
				if (e.RemovedItems != null)
				{
					DeselectItemsCommand?.TryExecute(e.RemovedItems);
				}
			}
		}

		private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
		{
			QuerySubmittedCommand?.TryExecute(args.QueryText);
		}

		private void OnToolbarClick(object sender, ToolbarButtonClickEventArgs e)
		{
			switch (e.ClickedButton)
			{
				case ToolbarButton.New:
					NewCommand?.TryExecute();
					break;
				case ToolbarButton.Delete:
					DeleteCommand?.TryExecute();
					break;
				case ToolbarButton.Select:
					StartSelectionCommand?.TryExecute();
					break;
				case ToolbarButton.Refresh:
					RefreshCommand?.TryExecute();
					break;
				case ToolbarButton.Cancel:
					CancelSelectionCommand?.TryExecute();
					break;
			}
		}

		#region NotifyPropertyChanged
		public void NotifyPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
