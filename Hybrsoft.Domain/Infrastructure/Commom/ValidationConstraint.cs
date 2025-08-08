using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Enums;
using System;

namespace Hybrsoft.Domain.Infrastructure.Commom
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
		public virtual string Message => _resourceService?.GetString(nameof(ResourceFiles.ValidationErrors), _messageKey) ?? _messageKey;

		public void SetResourceService(IResourceService resourceService)
		{
			_resourceService = resourceService;
		}
	}

	public class RequiredConstraint<T>(string propertyName, Func<T, object> propertyValue)
		: ValidationConstraint<T>("ValidationConstraint_RequiredError", model => ValidateProperty(model, propertyValue))
	{
		public string PropertyName { get; set; } = propertyName;
		public Func<T, object> PropertyValue { get; set; } = propertyValue;

		private static bool ValidateProperty(T model, Func<T, object> propertyValue)
		{
			var value = propertyValue(model);
			return value != null && !string.IsNullOrEmpty(value.ToString());
		}

		public override string Message => _resourceService?.GetString(nameof(ResourceFiles.ValidationErrors), MessageKey)
			?.Replace("{PropertyName}", PropertyName) ?? $"Property '{PropertyName}' cannot be empty.";
	}

	public class RequiredGreaterThanZeroConstraint<T>(string propertyName, Func<T, object> propertyValue)
		: ValidationConstraint<T>("ValidationConstraint_RequiredGreaterThanZeroError", model => ValidateProperty(model, propertyValue))
	{
		public string PropertyName { get; set; } = propertyName;
		public Func<T, object> PropertyValue { get; set; } = propertyValue;

		private static bool ValidateProperty(T model, Func<T, object> propertyValue)
		{
			var value = propertyValue(model);
			if (value != null)
			{
				if (Double.TryParse(value.ToString(), out double d))
				{
					return d > 0;
				}
			}
			return true;
		}

		public override string Message => _resourceService?.GetString(nameof(ResourceFiles.ValidationErrors), MessageKey)
			?.Replace("{PropertyName}", PropertyName) ?? $"Property '{PropertyName}' cannot be empty.";
	}

	public class PositiveConstraint<T>(string propertyName, Func<T, object> propertyValue)
		: ValidationConstraint<T>("ValidationConstraint_PositiveError", model => ValidateProperty(model, propertyValue))
	{
		public string PropertyName { get; set; } = propertyName;
		public Func<T, object> PropertyValue { get; set; } = propertyValue;

		private static bool ValidateProperty(T model, Func<T, object> propertyValue)
		{
			var value = propertyValue(model);
			if (value != null)
			{
				if (Double.TryParse(value.ToString(), out double d))
				{
					return d >= 0;
				}
			}
			return true;
		}

		public override string Message => _resourceService?.GetString(nameof(ResourceFiles.ValidationErrors), MessageKey)
			?.Replace("{PropertyName}", PropertyName) ?? $"Property '{PropertyName}' must be positive.";
	}

	public class NonZeroConstraint<T>(string propertyName, Func<T, object> propertyValue)
		: ValidationConstraint<T>("ValidationConstraint_NonZeroError", model => ValidateProperty(model, propertyValue))
	{
		public string PropertyName { get; set; } = propertyName;
		public Func<T, object> PropertyValue { get; set; } = propertyValue;

		private static bool ValidateProperty(T model, Func<T, object> propertyValue)
		{
			var value = propertyValue(model);
			if (value != null)
			{
				if (Double.TryParse(value.ToString(), out double d))
				{
					return d != 0;
				}
			}
			return true;
		}

		public override string Message => _resourceService?.GetString(nameof(ResourceFiles.ValidationErrors), MessageKey)
			?.Replace("{PropertyName}", PropertyName) ?? $"Property '{PropertyName}' cannot be zero";
	}

	public class GreaterThanConstraint<T>(string propertyName, Func<T, object> propertyValue, double value)
		: ValidationConstraint<T>("ValidationConstraint_GreaterThanError", model => ValidateProperty(model, propertyValue, value))
	{
		public string PropertyName { get; set; } = propertyName;
		public Func<T, object> PropertyValue { get; set; } = propertyValue;
		public double Value { get; set; } = value;

		private static bool ValidateProperty(T model, Func<T, object> propertyValue, double value)
		{
			var val = propertyValue(model);
			if (val != null)
			{
				if (Double.TryParse(value.ToString(), out double d))
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

	public class NonGreaterThanConstraint<T> : ValidationConstraint<T>
	{
		public NonGreaterThanConstraint(string propertyName, Func<T, object> propertyValue, double value, string valueDesc = null)
			: base("ValidationConstraint_NonGreaterThanError", model => ValidateProperty(model, propertyValue, value))
		{
			PropertyName = propertyName;
			PropertyValue = propertyValue;
			Value = value;
			ValueDesc = valueDesc ?? Value.ToString();
		}

		public string PropertyName { get; set; }
		public Func<T, object> PropertyValue { get; set; }
		public double Value { get; set; }
		public string ValueDesc { get; set; }

		private static bool ValidateProperty(T model, Func<T, object> propertyValue, double value)
		{
			var val = propertyValue(model);
			if (val != null)
			{
				if (Double.TryParse(value.ToString(), out double d))
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
		public string PropertyName { get; set; } = propertyName;
		public Func<T, object> PropertyValue { get; set; } = propertyValue;
		public double Value { get; set; } = value;

		private static bool ValidateProperty(T model, Func<T, object> propertyValue, double value)
		{
			var val = propertyValue(model);
			if (val != null)
			{
				if (Double.TryParse(value.ToString(), out double d))
				{
					return d < value;
				}
			}
			return true;
		}

		public override string Message => _resourceService?.GetString(nameof(ResourceFiles.ValidationErrors), MessageKey)
			?.Replace("{PropertyName}", PropertyName)
			.Replace("{Value}", Value.ToString()) ?? $"Property '{PropertyName}' must be less than {Value}.";
	}

	public class EmailValidationConstraint<T>(string propertyName, Func<T, string> propertyValue)
		: ValidationConstraint<T>("ValidationConstraint_EmailValidationError", model => ValidateProperty(model, propertyValue))
	{
		public string PropertyName { get; set; } = propertyName;
		public Func<T, string> PropertyValue { get; set; } = propertyValue;
		private static readonly string _pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

		private static bool ValidateProperty(T model, Func<T, string> propertyValue)
		{
			var value = propertyValue(model);
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			return System.Text.RegularExpressions.Regex.IsMatch(value, _pattern);
		}

		public override string Message => _resourceService?.GetString(nameof(ResourceFiles.ValidationErrors), MessageKey)
			?.Replace("{PropertyName}", PropertyName) ?? $"Property '{PropertyName}' is not a valid email address.";
	}

	public class AlphanumericValidationConstraint<T>(string propertyName, Func<T, string> propertyValue)
		: ValidationConstraint<T>("ValidationConstraint_AlphanumericValidationError", model => ValidateProperty(model, propertyValue))
	{
		public string PropertyName { get; set; } = propertyName;
		public Func<T, string> PropertyValue { get; set; } = propertyValue;
		private static readonly string _pattern = @"^[a-zA-Z0-9]*$";

		private static bool ValidateProperty(T model, Func<T, string> propertyValue)
		{
			var value = propertyValue(model);
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			return System.Text.RegularExpressions.Regex.IsMatch(value, _pattern);
		}

		public override string Message => _resourceService?.GetString(nameof(ResourceFiles.ValidationErrors), MessageKey)
			?.Replace("{PropertyName}", PropertyName) ?? $"Property '{PropertyName}' must be alphanumeric.";
	}
}
