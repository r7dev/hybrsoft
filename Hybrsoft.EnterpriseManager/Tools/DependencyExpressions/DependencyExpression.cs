namespace Hybrsoft.EnterpriseManager.Tools.DependencyExpressions
{
	public class DependencyExpression(string name, string[] dependencies)
	{
		public string Name { get; } = name;
		public string[] Dependencies { get; } = dependencies;
	}
}
