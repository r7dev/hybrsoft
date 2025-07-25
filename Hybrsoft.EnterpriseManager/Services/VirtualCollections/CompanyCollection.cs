﻿using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Common.VirtualCollection;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services.VirtualCollections
{
	public partial class CompanyCollection(ICompanyService companyService, ILogService logService) : VirtualCollection<CompanyDto>(logService)
	{
		private DataRequest<Company> _dataRequest = null;

		public ICompanyService CompanyService { get; } = companyService;

		private readonly CompanyDto _defaultItem = CompanyDto.CreateEmpty();
		protected override CompanyDto DefaultItem => _defaultItem;

		public async Task LoadAsync(DataRequest<Company> dataRequest)
		{
			try
			{
				_dataRequest = dataRequest;
				Count = await CompanyService.GetCompaniesCountAsync(_dataRequest);
				Ranges[0] = await CompanyService.GetCompaniesAsync(0, RangeSize, _dataRequest);
			}
			catch (Exception)
			{
				Count = 0;
				throw;
			}
		}

		protected override async Task<IList<CompanyDto>> FetchDataAsync(int rangeIndex, int rangeSize)
		{
			try
			{
				return await CompanyService.GetCompaniesAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
			}
			catch (Exception ex)
			{
				LogException("CompanyCollection", "Fetch", ex);
			}
			return null;
		}
	}
}
