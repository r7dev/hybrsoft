﻿using System.Windows.Input;

namespace Hybrsoft.EnterpriseManager.Extensions
{
	static public class ICommandExtensions
	{
		static public void TryExecute(this ICommand command, object parameter = null)
		{
			if (command != null)
			{
				if (command.CanExecute(parameter))
				{
					command.Execute(parameter);
				}
			}
		}
	}
}
