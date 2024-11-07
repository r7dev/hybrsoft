
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.Infrastructure.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure.LogService
{
	public class LogService : ILogService
	{
		public LogService(IDataServiceFactory dataServiceFactory, IMessageService messageService)
		{
			DataServiceFactory = dataServiceFactory;
			MessageService = messageService;
		}

		public IDataServiceFactory DataServiceFactory { get; }
		public IMessageService MessageService { get; }

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
			var appLog = new AppLog()
			{
				User = AppSettings.Current.UserName ?? "App",
				Type = type,
				Source = source,
				Action = action,
				Message = message,
				Description = description,
			};

			appLog.IsRead = type != LogType.Error;

			await CreateLogAsync(appLog);
			MessageService.Send(this, "LogAdded", appLog);
		}

		public async Task<int> CreateLogAsync(AppLog appLog)
		{
			using (var dataService = DataServiceFactory.CreateDataService())
			{
				return await dataService.CreateLogAsync(appLog);
			}
		}
	}
}
