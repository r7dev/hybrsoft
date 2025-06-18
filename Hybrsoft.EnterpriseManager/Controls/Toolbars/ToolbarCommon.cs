using System;

namespace Hybrsoft.EnterpriseManager.Controls
{
	public enum ToolbarButton
	{
		Back,
		New,
		Edit,
		Delete,
		Cancel,
		Save,
		Select,
		Refresh,
		Accept
	}

	public enum ListToolbarMode
	{
		Default,
		Cancel,
		CancelMore
	}

	public enum DetailToolbarMode
	{
		Default,
		BackEditdDelete,
		CancelSave
	}

	public class ToolbarButtonClickEventArgs(ToolbarButton button) : EventArgs
	{
		public ToolbarButton ClickedButton { get; } = button;
	}

	public delegate void ToolbarButtonClickEventHandler(object sender, ToolbarButtonClickEventArgs e);
}
