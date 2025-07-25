using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Hybrsoft.EnterpriseManager.Tools.DependencyExpressions;
using System.ComponentModel;
using System.Windows.Input;
using Hybrsoft.EnterpriseManager.Extensions;
using Hybrsoft.EnterpriseManager.Tools.ElementSet;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Controls
{
	public sealed partial class Details : UserControl, INotifyExpressionChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		static private readonly DependencyExpressions DependencyExpressions = new();

		public Details()
		{
			this.InitializeComponent();
			Loaded += OnLoaded;
			DependencyExpressions.Initialize(this);
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			foreach (var ctrl in GetFormControls())
			{
				ctrl.VisualStateChanged += OnVisualStateChanged;
			}
			UpdateEditMode();
		}

		#region CanGoBack*
		public bool CanGoBack
		{
			get { return (bool)GetValue(CanGoBackProperty); }
			set { SetValue(CanGoBackProperty, value); }
		}

		private static void CanGoBackChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as Details;
			DependencyExpressions.UpdateDependencies(control, nameof(CanGoBack));
		}

		public static readonly DependencyProperty CanGoBackProperty = DependencyProperty.Register(nameof(CanGoBack), typeof(bool), typeof(Details), new PropertyMetadata(false, CanGoBackChanged));
		#endregion

		#region IsEditMode*
		public bool IsEditMode
		{
			get { return (bool)GetValue(IsEditModeProperty); }
			set { SetValue(IsEditModeProperty, value); }
		}

		private static void IsEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as Details;
			DependencyExpressions.UpdateDependencies(control, nameof(IsEditMode));
			control.UpdateEditMode();
		}

		public static readonly DependencyProperty IsEditModeProperty = DependencyProperty.Register(nameof(IsEditMode), typeof(bool), typeof(Details), new PropertyMetadata(false, IsEditModeChanged));
		#endregion

		#region DetailsContent
		public object DetailsContent
		{
			get { return (object)GetValue(DetailsContentProperty); }
			set { SetValue(DetailsContentProperty, value); }
		}

		public static readonly DependencyProperty DetailsContentProperty = DependencyProperty.Register(nameof(DetailsContent), typeof(object), typeof(Details), new PropertyMetadata(null));
		#endregion

		#region DetailsTemplate
		public DataTemplate DetailsTemplate
		{
			get { return (DataTemplate)GetValue(DetailsTemplateProperty); }
			set { SetValue(DetailsTemplateProperty, value); }
		}

		public static readonly DependencyProperty DetailsTemplateProperty = DependencyProperty.Register(nameof(DetailsTemplate), typeof(DataTemplate), typeof(Details), new PropertyMetadata(null));
		#endregion

		#region DefaultCommands
		public string DefaultCommands
		{
			get { return (string)GetValue(DefaultCommandsProperty); }
			set { SetValue(DefaultCommandsProperty, value); }
		}

		public static readonly DependencyProperty DefaultCommandsProperty = DependencyProperty.Register(nameof(DefaultCommands), typeof(string), typeof(Details), new PropertyMetadata("edit,delete"));
		#endregion


		#region BackCommand
		public ICommand BackCommand
		{
			get { return (ICommand)GetValue(BackCommandProperty); }
			set { SetValue(BackCommandProperty, value); }
		}

		public static readonly DependencyProperty BackCommandProperty = DependencyProperty.Register(nameof(BackCommand), typeof(ICommand), typeof(Details), new PropertyMetadata(null));
		#endregion

		#region EditCommand
		public ICommand EditCommand
		{
			get { return (ICommand)GetValue(EditCommandProperty); }
			set { SetValue(EditCommandProperty, value); }
		}

		public static readonly DependencyProperty EditCommandProperty = DependencyProperty.Register(nameof(EditCommand), typeof(ICommand), typeof(Details), new PropertyMetadata(null));
		#endregion

		#region DeleteCommand
		public ICommand DeleteCommand
		{
			get { return (ICommand)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}

		public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(Details), new PropertyMetadata(null));
		#endregion

		#region SaveCommand
		public ICommand SaveCommand
		{
			get { return (ICommand)GetValue(SaveCommandProperty); }
			set { SetValue(SaveCommandProperty, value); }
		}

		public static readonly DependencyProperty SaveCommandProperty = DependencyProperty.Register(nameof(SaveCommand), typeof(ICommand), typeof(Details), new PropertyMetadata(null));
		#endregion

		#region CancelCommand
		public ICommand CancelCommand
		{
			get { return (ICommand)GetValue(CancelCommandProperty); }
			set { SetValue(CancelCommandProperty, value); }
		}

		public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register(nameof(CancelCommand), typeof(ICommand), typeof(Details), new PropertyMetadata(null));
		#endregion

		#region CancelSecondaryCommand
		public ICommand CancelSecondaryCommand
		{
			get { return (ICommand)GetValue(CancelSecondaryCommandProperty); }
			set { SetValue(CancelSecondaryCommandProperty, value); }
		}

		public static readonly DependencyProperty CancelSecondaryCommandProperty = DependencyProperty.Register(nameof(CancelSecondaryCommand), typeof(ICommand), typeof(Details), new PropertyMetadata(null));
		#endregion

		public DetailToolbarMode ToolbarMode
		{
			get
			{
				if (IsEditMode)
				{
					return DetailToolbarMode.CancelSave;
				}
				return CanGoBack ? DetailToolbarMode.BackEditdDelete : DetailToolbarMode.Default;
			}
		}
		public static readonly DependencyExpression ToolbarModeExpression = DependencyExpressions.Register(
			nameof(ToolbarMode),
			nameof(IsEditMode),
			nameof(CanGoBack)
		);

		private void OnToolbarClick(object sender, ToolbarButtonClickEventArgs e)
		{
			switch (e.ClickedButton)
			{
				case ToolbarButton.Back:
					BackCommand?.TryExecute();
					break;
				case ToolbarButton.Edit:
					EditCommand?.TryExecute();
					break;
				case ToolbarButton.Delete:
					DeleteCommand?.TryExecute();
					break;
				case ToolbarButton.Save:
					SaveCommand?.TryExecute();
					break;
				case ToolbarButton.Cancel:
					CancelCommand?.TryExecute();
					break;
				case ToolbarButton.CancelSecondary:
					CancelSecondaryCommand?.TryExecute();
					break;
			}
		}

		public void SetFocus()
		{
			GetFormControls().FirstOrDefault()?.Focus(FocusState.Programmatic);
		}

		private void OnVisualStateChanged(object sender, FormVisualState e)
		{
			if (e == FormVisualState.Focused)
			{
				if (!IsEditMode)
				{
					EditCommand?.TryExecute();
				}
			}
		}

		private void UpdateEditMode()
		{
			if (IsEditMode)
			{
				foreach (var ctrl in GetFormControls().Where(r => r.VisualState != FormVisualState.Focused))
				{
					ctrl.SetVisualState(FormVisualState.Ready);
				}
			}
			else
			{
				Focus(FocusState.Programmatic);
				foreach (var ctrl in GetFormControls())
				{
					ctrl.SetVisualState(FormVisualState.Idle);
				}
			}
		}

		private IEnumerable<IFormControl> GetFormControls()
		{
			return ElementSet.Children<Control>(container)
				.Where(r =>
				{
					if (r is IFormControl ctrl)
					{
						return true;
					}
					return false;
				})
				.Cast<IFormControl>();
		}

		#region NotifyPropertyChanged
		public void NotifyPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
