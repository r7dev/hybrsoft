using Microsoft.UI.Xaml;
using System;

namespace Hybrsoft.EnterpriseManager.Controls
{
	public interface IFormControl
	{
		event EventHandler<FormVisualState> VisualStateChanged;

		FormEditMode Mode { get; }
		FormVisualState VisualState { get; }

		bool IsEnabled { get; }

		bool Focus(FocusState value);

		void SetVisualState(FormVisualState visualState);
	}
}
