﻿using Hybrsoft.Enums;
using Hybrsoft.UI.Windows.Services;
using System.Linq;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class AuthorizationService(ILookupTables lookupTables) : IAuthorizationService
	{
		private ILookupTables LookupTables { get; } = lookupTables;

		public bool HasPermission(Permissions permission)
		{
			return LookupTables.Permissions.Any(r => r.Name == permission.ToString());
		}
	}
}
