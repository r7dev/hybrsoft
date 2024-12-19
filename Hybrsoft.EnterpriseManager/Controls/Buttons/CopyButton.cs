﻿using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml;

namespace Hybrsoft.EnterpriseManager.Controls
{
	public sealed partial class CopyButton : Button
	{
		public CopyButton()
		{
			this.DefaultStyleKey = typeof(CopyButton);
		}

		private void CopyButton_Click(object sender, RoutedEventArgs e)
		{
			if (GetTemplateChild("CopyToClipboardSuccessAnimation") is Storyboard _storyBoard)
			{
				_storyBoard.Begin();
			}
		}

		protected override void OnApplyTemplate()
		{
			Click -= CopyButton_Click;
			base.OnApplyTemplate();
			Click += CopyButton_Click;
		}
	}
}
