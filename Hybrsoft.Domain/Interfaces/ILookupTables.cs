using Hybrsoft.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.Interfaces
{
	public interface ILookupTables
	{
		Task InitializeAsync();

		IList<ScheduleTypeDto> ScheduleTypes { get; }

		string GetScheduleType(Int16 scheduleTypeID);
	}

	public class LookupTablesProxy
	{
		static public ILookupTables Instance { get; set; }
	}
}
