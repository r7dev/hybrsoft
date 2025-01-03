using System;

namespace Hybrsoft.Infrastructure.Common
{
	static class UIDGenerator
	{
		static private readonly DateTime DateSeed = DateTime.Parse("2025/01/01");

		static public long Next(int prefix = 1)
		{
			return (long)(DateTime.UtcNow - DateSeed).TotalMilliseconds + prefix * 100000000000;
		}
	}
}
