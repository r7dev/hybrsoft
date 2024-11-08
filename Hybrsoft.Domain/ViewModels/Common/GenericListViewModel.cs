using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Hybrsoft.Domain.ViewModels
{
	abstract public partial class GenericListViewModel<TModel> : ViewModelBase where TModel : ObservableObject
	{
		public GenericListViewModel(ICommonServices commonServices) : base(commonServices)
		{
		}

		public override string Title => String.IsNullOrEmpty(Query) ? $" ({ItemsCount})" : $" ({ItemsCount} for \"{Query}\")";

		private IList<TModel> _items = null;
		public IList<TModel> Items
		{
			get => _items;
			set => Set(ref _items, value);
		}

		private int _itemsCount = 0;
		public int ItemsCount
		{
			get => _itemsCount;
			set => Set(ref _itemsCount, value);
		}

		private TModel _selectedItem = default(TModel);
		public TModel SelectedItem
		{
			get => _selectedItem;
			set
			{
				if (Set(ref _selectedItem, value))
				{
					if (!IsMultipleSelection)
					{
						MessageService.Send(this, "ItemSelected", _selectedItem);
					}
				}
			}
		}

		private string _query = null;
		public string Query
		{
			get => _query;
			set => Set(ref _query, value);
		}

		private bool _isMultipleSelection = false;
		public bool IsMultipleSelection
		{
			get => _isMultipleSelection;
			set => Set(ref _isMultipleSelection, value);
		}

		public List<TModel> SelectedItems { get; protected set; }
		public IndexRange[] SelectedIndexRanges { get; protected set; }

		public ICommand NewCommand => new RelayCommand(OnNew);

		public ICommand RefreshCommand => new RelayCommand(OnRefresh);

		public ICommand StartSelectionCommand => new RelayCommand(OnStartSelection);
		virtual protected void OnStartSelection()
		{
			StatusMessage("Start selection");
			SelectedItem = null;
			SelectedItems = new List<TModel>();
			SelectedIndexRanges = null;
			IsMultipleSelection = true;
		}

		public ICommand CancelSelectionCommand => new RelayCommand(OnCancelSelection);
		virtual protected void OnCancelSelection()
		{
			StatusReady();
			SelectedItems = null;
			SelectedIndexRanges = null;
			IsMultipleSelection = false;
			SelectedItem = Items?.FirstOrDefault();
		}

		public ICommand SelectItemsCommand => new RelayCommand<IList<object>>(OnSelectItems);
		virtual protected void OnSelectItems(IList<object> items)
		{
			StatusReady();
			if (IsMultipleSelection)
			{
				SelectedItems.AddRange(items.Cast<TModel>());
				StatusMessage($"{SelectedItems.Count} items selected");
			}
		}

		public ICommand DeselectItemsCommand => new RelayCommand<IList<object>>(OnDeselectItems);
		virtual protected void OnDeselectItems(IList<object> items)
		{
			if (items?.Count > 0)
			{
				StatusReady();
			}
			if (IsMultipleSelection)
			{
				foreach (TModel item in items)
				{
					SelectedItems.Remove(item);
				}
				StatusMessage($"{SelectedItems.Count} items selected");
			}
		}

		public ICommand SelectRangesCommand => new RelayCommand<IndexRange[]>(OnSelectRanges);
		virtual protected void OnSelectRanges(IndexRange[] indexRanges)
		{
			SelectedIndexRanges = indexRanges;
			int count = SelectedIndexRanges?.Sum(r => r.Length) ?? 0;
			StatusMessage($"{count} items selected");
		}

		public ICommand DeleteSelectionCommand => new RelayCommand(OnDeleteSelection);

		abstract protected void OnNew();
		abstract protected void OnRefresh();
		abstract protected void OnDeleteSelection();
	}
}
