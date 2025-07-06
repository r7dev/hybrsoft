using Hybrsoft.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.Interfaces
{
	public interface ILookupTables
	{
		Task InitializeAsync();

		IList<CountryDto> Countries { get; }
		IList<ScheduleTypeDto> ScheduleTypes { get; }
		IList<RelativeTypeDto> RelativeTypes { get; }

		string GetCountry(Int16 countryID);
		string GetScheduleType(Int16 scheduleTypeID);
		string GetRelativeType(Int16 relativeTypeID);
	}

	public class LookupTablesProxy
	{
		static public ILookupTables Instance { get; set; }
	}
}
