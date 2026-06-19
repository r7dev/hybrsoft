using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure.LogService
{
	public class LogService(IDataServiceFactory dataServiceFactory,
		IContextService contextService,
		IEmbeddingService embeddingService,
		IMessageService messageService) : ILogService
	{
		private readonly IDataServiceFactory _dataServiceFactory = dataServiceFactory;
		private readonly IContextService _contextService = contextService;
		private readonly IEmbeddingService _embeddingService = embeddingService;
		private readonly IMessageService _messageService = messageService;
		readonly int _maxLength = 4000;

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
			string user = AppSettings.Current.UserName ?? "App";
			string refinedDescription = description?.Length > _maxLength
				? description[.._maxLength]
				: description;
			string searchTerms = BuildSearchTerms(user, type, source, action, message, refinedDescription);
			var appLog = new AppLog
			{
				User = user,
				Type = type,
				Source = source,
				Action = action,
				Message = message,
				Description = refinedDescription,
				AppType = AppType.EnterpriseManager,
				SearchTerms = searchTerms,
				IsRead = type != LogType.Error
			};

			await CreateLogAsync(appLog);
			_messageService.Send(this, "LogAdded", appLog);

			await CreateLogEmbeddingAsync(appLog);
		}

		private string BuildSearchTerms(string user, LogType type, string source, string action, string message, string description)
		{
			string searchTerms = $"{user} {type.GetDescription()} {source} {action} {message} {description}";
			return searchTerms.Length > _maxLength ? searchTerms[.._maxLength] : searchTerms;
		}

		public async Task<AppLogModel> GetLogAsync(long id)
		{
			using var dataService = _dataServiceFactory.CreateDataService();
			var item = await dataService.GetAppLogAsync(id);
			if (item != null)
			{
				return CreateAppLogModel(item);
			}
			return null;
		}

		public async Task<IList<AppLogModel>> GetLogsAsync(DataRequest<AppLog> request)
		{
			var collection = new LogCollection(this);
			await collection.LoadAsync(request);
			return collection;
		}

		public async Task<IList<AppLogModel>> GetLogsAsync(int skip, int take, DataRequest<AppLog> request)
		{
			var models = new List<AppLogModel>();
			using var dataSource = _dataServiceFactory.CreateDataService();
			var items = await dataSource.GetAppLogsAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(CreateAppLogModel(item));
			}
			return models;
		}

		public async Task<int> GetLogsCountAsync(DataRequest<AppLog> request)
		{
			using var dataSource = _dataServiceFactory.CreateDataService();
			return await dataSource.GetAppLogsCountAsync(request);
		}

		public async Task<int> CreateLogAsync(AppLog model)
		{
			using var dataService = _dataServiceFactory.CreateDataService();
			return await dataService.CreateAppLogAsync(model);
		}

		public async Task<int> CreateLogEmbeddingAsync(AppLog model)
		{
			long id = model.AppLogID;
			if (id > 0 && AppSettings.Current.UseSemanticSearch && _embeddingService.IsConfigured)
			{
				await _contextService.RunAsync(async () => {
					var embedding = new AppLogEmbedding
					{
						AppLogEmbeddingID = id,
						Embedding = await _embeddingService.GenerateEmbeddingAsync(model.SearchTerms)
					};
					using var dataService = _dataServiceFactory.CreateDataService();
					await dataService.CreateAppLogEmbeddingAsync(embedding);
				});
			}
			return 0;
		}

		public async Task<int> DeleteLogAsync(AppLogModel model)
		{
			var item = new AppLog { AppLogID = model.AppLogID };
			using var dataSource = _dataServiceFactory.CreateDataService();
			return await dataSource.DeleteAppLogsAsync(item);
		}

		public async Task<int> DeleteLogRangeAsync(int index, int length, DataRequest<AppLog> request)
		{
			using var ds = _dataServiceFactory.CreateDataService();
			var items = await ds.GetAppLogKeysAsync(index, length, request);
			return await ds.DeleteAppLogsAsync([.. items]);
		}

		public async Task MarkAllAsReadAsync()
		{
			using var dataSource = _dataServiceFactory.CreateDataService();
			await dataSource.MarkAllAsReadAsync();
		}

		private static AppLogModel CreateAppLogModel(AppLog source)
		{
			return new AppLogModel()
			{
				AppLogID = source.AppLogID,
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
