using Hybrsoft.Infrastructure.Enums;
using Hybrsoft.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Hybrsoft.Infrastructure.DataServices.Base
{
	partial class DataServiceBase
	{
		public IList<NavigationItem> GetNavigationItemByAppType(AppType appType)
		{
			return [.. _universalDataSource.NavigationItems
				.Where(f => f.AppType == appType)
				.AsNoTracking()];
		}
	}
}
