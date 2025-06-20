﻿using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Infrastructure.Enums;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.Domain.ViewModels
{
	abstract public partial class GenericDetailsViewModel<TModel>(ICommonServices commonServices) : ViewModelBase(commonServices) where TModel : ObservableObject, new()
	{
		public ILookupTables LookupTables => LookupTablesProxy.Instance;

		public bool IsDataAvailable => _item != null;
		public bool IsDataUnavailable => !IsDataAvailable;

		public bool CanGoBack => !IsMainView && NavigationService.CanGoBack;
		public bool BackAfterSave = false;

		private TModel _item = null;
		public TModel Item
		{
			get => _item;
			set
			{
				if (Set(ref _item, value))
				{
					EditableItem = _item;
					IsEnabled = (!_item?.IsEmpty) ?? false;
					NotifyPropertyChanged(nameof(IsDataAvailable));
					NotifyPropertyChanged(nameof(IsDataUnavailable));
					NotifyPropertyChanged(nameof(Title));
				}
			}
		}

		private TModel _editableItem = null;
		public TModel EditableItem
		{
			get => _editableItem;
			set => Set(ref _editableItem, value);
		}

		private bool _isEditMode = false;
		public bool IsEditMode
		{
			get => _isEditMode;
			set => Set(ref _isEditMode, value);
		}

		private bool _isEnabled = true;
		public bool IsEnabled
		{
			get => _isEnabled;
			set => Set(ref _isEnabled, value);
		}

		public ICommand BackCommand => new RelayCommand(OnBack);
		virtual protected void OnBack()
		{
			StatusReady();
			if (NavigationService.CanGoBack)
			{
				NavigationService.GoBack();
			}
		}

		public ICommand EditCommand => new RelayCommand(OnEdit);
		virtual protected void OnEdit()
		{
			StatusReady();
			BeginEdit();
			MessageService.Send(this, "BeginEdit", Item);
		}
		virtual public void BeginEdit()
		{
			if (!IsEditMode)
			{
				IsEditMode = true;
				// Create a copy for edit
				var editableItem = new TModel();
				editableItem.Merge(Item);
				EditableItem = editableItem;
			}
		}

		public ICommand CancelCommand => new RelayCommand(OnCancel);
		virtual protected void OnCancel()
		{
			StatusReady();
			CancelEdit();
			MessageService.Send(this, "CancelEdit", Item);
		}
		virtual public void CancelEdit()
		{
			if (ItemIsNew)
			{
				// We were creating a new item: cancel means exit
				if (NavigationService.CanGoBack)
				{
					NavigationService.GoBack();
				}
				else
				{
					NavigationService.CloseViewAsync();
				}
				return;
			}

			// We were editing an existing item: just cancel edition
			if (IsEditMode)
			{
				EditableItem = Item;
			}
			IsEditMode = false;
		}

		public ICommand SaveCommand => new RelayCommand(OnSave);
		virtual protected async void OnSave()
		{
			StatusReady();
			var result = Validate(EditableItem);
			if (result.IsOk)
			{
				await SaveAsync();
			}
			else
			{
				string resourceKey = string.Concat(nameof(GenericDetailsViewModel<TModel>), "_0PleaseCorrectTheErrorAndTryAgain");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKey);
				string content = string.Format(resourceValue, result.Description);
				await DialogService.ShowAsync(result.Message, content);
			}
		}
		virtual public async Task SaveAsync()
		{
			IsEnabled = false;
			bool isNew = ItemIsNew;
			if (await SaveItemAsync(EditableItem))
			{
				Item.Merge(EditableItem);
				Item.NotifyChanges();
				NotifyPropertyChanged(nameof(Title));
				EditableItem = Item;

				if (isNew)
				{
					MessageService.Send(this, "NewItemSaved", Item);
				}
				else
				{
					MessageService.Send(this, "ItemChanged", Item);
				}
				IsEditMode = false;

				NotifyPropertyChanged(nameof(ItemIsNew));
			}
			IsEnabled = true;
			if (isNew && BackAfterSave)
			{
				await Task.Delay(3000);
				BackCommand?.Execute(null);
			}
		}

		public ICommand DeleteCommand => new RelayCommand(OnDelete);
		virtual protected async void OnDelete()
		{
			StatusReady();
			if (await ConfirmDeleteAsync())
			{
				await DeleteAsync();
			}
		}
		virtual public async Task DeleteAsync()
		{
			var model = Item;
			if (model != null)
			{
				IsEnabled = false;
				if (await DeleteItemAsync(model))
				{
					MessageService.Send(this, "ItemDeleted", model);
				}
				else
				{
					IsEnabled = true;
				}
			}
		}

		virtual public Result Validate(TModel model)
		{
			StringBuilder errors = new();
			foreach (var constraint in GetValidationConstraints(model))
			{
				if (!constraint.Validate(model))
				{
					errors.AppendLine(constraint.Message);
				}
			}
			if (errors.Length > 0)
			{
				string message = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), string.Concat(nameof(GenericDetailsViewModel<TModel>), "_ValidationError"));
				return Result.Error(message, errors.ToString());
			}
			return Result.Ok();
		}

		virtual protected IEnumerable<IValidationConstraint<TModel>> GetValidationConstraints(TModel model) => [];

		abstract public bool ItemIsNew { get; }

		abstract protected Task<bool> SaveItemAsync(TModel model);
		abstract protected Task<bool> DeleteItemAsync(TModel model);
		abstract protected Task<bool> ConfirmDeleteAsync();
	}
}
