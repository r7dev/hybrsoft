using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
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

		public ICommand NewCommand => new RelayCommand(OnNew);

		public ICommand RefreshCommand => new RelayCommand(OnRefresh);

		abstract protected void OnNew();

		abstract protected void OnRefresh();
	}
}
