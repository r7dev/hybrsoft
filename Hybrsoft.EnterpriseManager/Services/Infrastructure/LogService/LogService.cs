using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure.LogService
{
	public class LogService(IDataServiceFactory dataServiceFactory, IMessageService messageService) : ILogService
	{
		public IDataServiceFactory DataServiceFactory { get; } = dataServiceFactory;
		public IMessageService MessageService { get; } = messageService;

		public async Task WriteAsync(LogType type, string source, string action, Exception ex)
		{
			await WriteAsync(LogType.Error, source, action, ex.Message, ex.ToString());
			Exception deepException = ex.InnerException;
			while (deepException != null)
			{
				await WriteAsync(LogType.Error, source, action, deepException.Message, deepException.ToString());
				deepException = deepException.InnerException;
			}
		}
		public async Task WriteAsync(LogType type, string source, string action, string message, string description)
		{
			int maxDescriptionLength = 4000;
			string refinedDescription = !string.IsNullOrEmpty(description) && description.Length > maxDescriptionLength
				? description[..maxDescriptionLength]
				: description;
			var appLog = new AppLog
			{
				User = AppSettings.Current.UserName ?? "App",
				Type = type,
				Source = source,
				Action = action,
				Message = message,
				Description = refinedDescription,
				AppType = AppType.EnterpriseManager,
				IsRead = type != LogType.Error
			};

			await CreateLogAsync(appLog);
			MessageService.Send(this, "LogAdded", appLog);
		}

		public async Task<AppLogDto> GetLogAsync(long id)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var item = await dataService.GetLogAsync(id);
			if (item != null)
			{
				return CreateAppLogModel(item);
			}
			return null;
		}

		public async Task<IList<AppLogDto>> GetLogsAsync(DataRequest<AppLog> request)
		{
			var collection = new LogCollection(this);
			await collection.LoadAsync(request);
			return collection;
		}

		public async Task<IList<AppLogDto>> GetLogsAsync(int skip, int take, DataRequest<AppLog> request)
		{
			var models = new List<AppLogDto>();
			using var dataSource = DataServiceFactory.CreateDataService();
			var items = await dataSource.GetLogsAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(CreateAppLogModel(item));
			}
			return models;
		}

		public async Task<int> GetLogsCountAsync(DataRequest<AppLog> request)
		{
			using var dataSource = DataServiceFactory.CreateDataService();
			return await dataSource.GetLogsCountAsync(request);
		}

		public async Task<int> CreateLogAsync(AppLog appLog)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.CreateLogAsync(appLog);
		}

		public async Task<int> DeleteLogAsync(AppLogDto model)
		{
			var appLog = new AppLog { AppLogId = model.AppLogId };
			using var dataSource = DataServiceFactory.CreateDataService();
			return await dataSource.DeleteLogsAsync(appLog);
		}

		public async Task<int> DeleteLogRangeAsync(int index, int length, DataRequest<AppLog> request)
		{
			using var ds = DataServiceFactory.CreateDataService();
			var items = await ds.GetLogKeysAsync(index, length, request);
			return await ds.DeleteLogsAsync([.. items]);
		}

		public async Task MarkAllAsReadAsync()
		{
			using var dataSource = DataServiceFactory.CreateDataService();
			await dataSource.MarkAllAsReadAsync();
		}

		private static AppLogDto CreateAppLogModel(AppLog source)
		{
			return new AppLogDto()
			{
				AppLogId = source.AppLogId,
				IsRead = source.IsRead,
				CreatedOn = source.CreateOn,
				User = source.User,
				Type = source.Type,
				Source = source.Source,
				Action = source.Action,
				Message = source.Message,
				Description = source.Description,
			};
		}
	}
}
