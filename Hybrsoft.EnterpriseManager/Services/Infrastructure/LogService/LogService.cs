using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.Infrastructure.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
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
			var appLog = new AppLog
			{
				User = AppSettings.Current.UserName ?? "App",
				Type = type,
				Source = source,
				Action = action,
				Message = message,
				Description = description,
				AppType = AppType.EnterpriseManager,
				IsRead = type != LogType.Error
			};

			await CreateLogAsync(appLog);
			MessageService.Send(this, "LogAdded", appLog);
		}

		public async Task<int> CreateLogAsync(AppLog appLog)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.CreateLogAsync(appLog);
		}
	}
}
