using Hybrsoft.Enums;
using Hybrsoft.UI.Windows.Services;
using System;
using System.Text.RegularExpressions;

namespace Hybrsoft.UI.Windows.Infrastructure.Common
{
	public interface IValidationConstraint<T>
	{
		Func<T, bool> Validate { get; }
		string Message { get; }
		void SetResourceService(IResourceService resourceService);
	}

	public class ValidationConstraint<T>(string messageKey, Func<T, bool> validate) : IValidationConstraint<T>
	{
		private readonly string _messageKey = messageKey;
		protected IResourceService _resourceService;
		protected string MessageKey => _messageKey;
		public Func<T, bool> Validate { get; set; } = validate;
		public virtual string Message => _resourceService?
			.GetString(nameof(ResourceFiles.ValidationErrors), _messageKey) ?? _messageKey;

		public void SetResourceService(IResourceService resourceService)
		{
			_resourceService = resourceService;
		}
	}

	public class RequiredConstraint<T>(string propertyName, Func<T, object> propertyValue)
		: ValidationConstraint<T>("ValidationConstraint_RequiredError", model => ValidateProperty(model, propertyValue))
	{
		public string PropertyName { get; } = propertyName;

		private static bool ValidateProperty(T model, Func<T, object> propertyValue)
		{
			var value = propertyValue(model);
			return value is not null && !string.IsNullOrEmpty(value.ToString());
		}

		public override string Message => _resourceService?
			.GetString(nameof(ResourceFiles.ValidationErrors), MessageKey)?
			.Replace("{PropertyName}", PropertyName);
	}

	public class RequiredGreaterThanZeroConstraint<T>(string propertyName, Func<T, object> propertyValue)
		: ValidationConstraint<T>("ValidationConstraint_RequiredGreaterThanZeroError", model => ValidateProperty(model, propertyValue))
	{
		public string PropertyName { get; } = propertyName;

		private static bool ValidateProperty(T model, Func<T, object> propertyValue)
		{
			var value = propertyValue(model);
			if (value is not null)
			{
				if (double.TryParse(value.ToString(), out double d))
				{
					return d > 0;
				}
			}
			return true;
		}

		public override string Message => _resourceService?
			.GetString(nameof(ResourceFiles.ValidationErrors), MessageKey)?
			.Replace("{PropertyName}", PropertyName);
	}

	public class PositiveConstraint<T>(string propertyName, Func<T, object> propertyValue)
		: ValidationConstraint<T>("ValidationConstraint_PositiveError", model => ValidateProperty(model, propertyValue))
	{
		public string PropertyName { get; } = propertyName;

		private static bool ValidateProperty(T model, Func<T, object> propertyValue)
		{
			var value = propertyValue(model);
			if (value is not null)
			{
				if (double.TryParse(value.ToString(), out double d))
				{
					return d >= 0;
				}
			}
			return true;
		}

		public override string Message => _resourceService?
			.GetString(nameof(ResourceFiles.ValidationErrors), MessageKey)?
			.Replace("{PropertyName}", PropertyName);
	}

	public class NonZeroConstraint<T>(string propertyName, Func<T, object> propertyValue)
		: ValidationConstraint<T>("ValidationConstraint_NonZeroError", model => ValidateProperty(model, propertyValue))
	{
		public string PropertyName { get; } = propertyName;

		private static bool ValidateProperty(T model, Func<T, object> propertyValue)
		{
			var value = propertyValue(model);
			if (value is not null)
			{
				if (double.TryParse(value.ToString(), out double d))
				{
					return d != 0;
				}
			}
			return true;
		}

		public override string Message => _resourceService?
			.GetString(nameof(ResourceFiles.ValidationErrors), MessageKey)?
			.Replace("{PropertyName}", PropertyName);
	}

	public class GreaterThanConstraint<T>(string propertyName, Func<T, object> propertyValue, double value)
		: ValidationConstraint<T>("ValidationConstraint_GreaterThanError", model => ValidateProperty(model, propertyValue, value))
	{
		public string PropertyName { get; } = propertyName;
		public double Value { get; } = value;

		private static bool ValidateProperty(T model, Func<T, object> propertyValue, double value)
		{
			var val = propertyValue(model);
			if (val != null)
			{
				if (double.TryParse(val.ToString(), out double d))
				{
					return d > value;
				}
			}
			return true;
		}

		public override string Message => _resourceService?.GetString(nameof(ResourceFiles.ValidationErrors), MessageKey)
			?.Replace("{PropertyName}", PropertyName)
			.Replace("{Value}", Value.ToString()) ?? $"Property '{PropertyName}' must be greater than {Value}.";
	}

	public class NonGreaterThanConstraint<T>(string propertyName, Func<T, object> propertyValue, double value, string valueDesc = null)
		: ValidationConstraint<T>("ValidationConstraint_NonGreaterThanError", model => ValidateProperty(model, propertyValue, value))
	{
		public string PropertyName { get; } = propertyName;
		public string ValueDesc { get; } = valueDesc ?? value.ToString();

		private static bool ValidateProperty(T model, Func<T, object> propertyValue, double value)
		{
			var val = propertyValue(model);
			if (val != null)
			{
				if (double.TryParse(val.ToString(), out double d))
				{
					return d <= value;
				}
			}
			return true;
		}

		public override string Message => _resourceService?.GetString(nameof(ResourceFiles.ValidationErrors), MessageKey)
			?.Replace("{PropertyName}", PropertyName)
			.Replace("{ValueDesc}", ValueDesc) ?? $"Property '{PropertyName}' cannot be greater than {ValueDesc}.";
	}

	public class LessThanConstraint<T>(string propertyName, Func<T, object> propertyValue, double value)
		: ValidationConstraint<T>("ValidationConstraint_LessThanError", model => ValidateProperty(model, propertyValue, value))
	{
		public string PropertyName { get; } = propertyName;
		public double Value { get; } = value;

		private static bool ValidateProperty(T model, Func<T, object> propertyValue, double value)
		{
			var val = propertyValue(model);
			if (val is not null)
			{
				if (double.TryParse(val.ToString(), out double d))
				{
					return d < value;
				}
			}
			return true;
		}

		public override string Message => _resourceService?
			.GetString(nameof(ResourceFiles.ValidationErrors), MessageKey)?
			.Replace("{PropertyName}", PropertyName)
			.Replace("{Value}", Value.ToString());
	}

	public class EmailValidationConstraint<T>(string propertyName, Func<T, string> propertyValue)
		: ValidationConstraint<T>("ValidationConstraint_EmailValidationError", model => ValidateProperty(model, propertyValue))
	{
		public string PropertyName { get; } = propertyName;

		private static readonly string _pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

		private static bool ValidateProperty(T model, Func<T, string> propertyValue)
		{
			var value = propertyValue(model);
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			return Regex.IsMatch(value, _pattern);
		}

		public override string Message => _resourceService?
			.GetString(nameof(ResourceFiles.ValidationErrors), MessageKey)?
			.Replace("{PropertyName}", PropertyName);
	}

	public class AlphanumericValidationConstraint<T>(string propertyName, Func<T, string> propertyValue)
		: ValidationConstraint<T>("ValidationConstraint_AlphanumericValidationError", model => ValidateProperty(model, propertyValue))
	{
		public string PropertyName { get; } = propertyName;

		private static readonly string _pattern = @"^[a-zA-Z0-9]*$";

		private static bool ValidateProperty(T model, Func<T, string> propertyValue)
		{
			var value = propertyValue(model);
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			return Regex.IsMatch(value, _pattern);
		}

		public override string Message => _resourceService?
			.GetString(nameof(ResourceFiles.ValidationErrors), MessageKey)?
			.Replace("{PropertyName}", PropertyName);
	}
}
