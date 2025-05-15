using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.EnterpriseManager.Extensions;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Controls
{
	public sealed partial class RoleSuggestBox : UserControl
	{
		public RoleSuggestBox()
		{
			if (!DesignMode.DesignModeEnabled)
			{
				RoleService = ServiceLocator.Current.GetService<IRoleService>();
			}
			this.InitializeComponent();
		}

		private IRoleService RoleService { get; }

		#region ExcludedRoleKeys
		public IList<long> ExcludedRoleKeys
		{
			get { return (IList<long>)GetValue(ExcludedRoleKeysProperty); }
			set { SetValue(ExcludedRoleKeysProperty, value); }
		}
		public static readonly DependencyProperty ExcludedRoleKeysProperty = DependencyProperty.Register(nameof(ExcludedRoleKeys), typeof(IList<long>), typeof(RoleSuggestBox), new PropertyMetadata(null));
		#endregion

		#region Items
		public IList<RoleDto> Items
		{
			get { return (IList<RoleDto>)GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}

		public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items), typeof(IList<RoleDto>), typeof(RoleSuggestBox), new PropertyMetadata(null));
		#endregion

		#region DisplayText
		public string DisplayText
		{
			get { return (string)GetValue(DisplayTextProperty); }
			set { SetValue(DisplayTextProperty, value); }
		}

		public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(nameof(DisplayText), typeof(string), typeof(RoleSuggestBox), new PropertyMetadata(null));
		#endregion

		#region IsReadOnly*
		public bool IsReadOnly
		{
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}

		private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as RoleSuggestBox;
			control.suggestBox.Mode = ((bool)e.NewValue == true) ? FormEditMode.ReadOnly : FormEditMode.Auto;
		}

		public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(RoleSuggestBox), new PropertyMetadata(false, IsReadOnlyChanged));
		#endregion

		#region RoleSelectedCommand
		public ICommand RoleSelectedCommand
		{
			get { return (ICommand)GetValue(RoleSelectedCommandProperty); }
			set { SetValue(RoleSelectedCommandProperty, value); }
		}

		public static readonly DependencyProperty RoleSelectedCommandProperty = DependencyProperty.Register(nameof(RoleSelectedCommand), typeof(ICommand), typeof(RoleSuggestBox), new PropertyMetadata(null));
		#endregion

		private async void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
		{
			if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
			{
				if (args.CheckCurrent())
				{
					Items = String.IsNullOrEmpty(sender.Text) ? null : await GetItems(sender.Text);
					if (String.IsNullOrEmpty(sender.Text) && !string.IsNullOrEmpty(DisplayText))
					{
						DisplayText = string.Empty;
						var argsChosen = (AutoSuggestBoxSuggestionChosenEventArgs)Activator.CreateInstance(typeof(AutoSuggestBoxSuggestionChosenEventArgs), true);
						OnSuggestionChosen(sender, argsChosen);
					}
				}
			}
		}

		private async Task<IList<RoleDto>> GetItems(string query)
		{
			var request = new DataRequest<Role>()
			{
				Query = query,
				Where = r => !ExcludedRoleKeys.Contains(r.RoleId),
				OrderBy = r => r.Name
			};
			return await RoleService.GetRolesAsync(0, 20, request);
		}

		private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
		{
			RoleSelectedCommand?.TryExecute(args.SelectedItem);
		}
	}
}
