using Hybrsoft.Infrastructure.Enums;
using System;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.Interfaces.Infrastructure
{
	public interface ILogService
	{
		Task WriteAsync(LogType type, string source, string action, string message, string description);
		Task WriteAsync(LogType type, string source, string action, Exception ex);
	}
}
