using System;

namespace Hybrsoft.Domain.Infrastructure.Commom
{
	public interface IValidationConstraint<T>
	{
		Func<T, bool> Validate { get; }
		string Message { get; }
	}

	public class ValidationConstraint<T>(string message, Func<T, bool> validate) : IValidationConstraint<T>
	{
		public Func<T, bool> Validate { get; set; } = validate;
		public string Message { get; set; } = message;
	}

	public class RequiredConstraint<T>(string propertyName, Func<T, object> propertyValue) : IValidationConstraint<T>
	{
		public string PropertyName { get; set; } = propertyName;
		public Func<T, object> PropertyValue { get; set; } = propertyValue;

		Func<T, bool> IValidationConstraint<T>.Validate => ValidateProperty;

		private bool ValidateProperty(T model)
		{
			var value = PropertyValue(model);
			if (value != null)
			{
				return !String.IsNullOrEmpty(value.ToString());
			}
			return false;
		}

		string IValidationConstraint<T>.Message => $"Property '{PropertyName}' cannot be empty.";
	}

	public class RequiredGreaterThanZeroConstraint<T>(string propertyName, Func<T, object> propertyValue) : IValidationConstraint<T>
	{
		public string PropertyName { get; set; } = propertyName;
		public Func<T, object> PropertyValue { get; set; } = propertyValue;

		Func<T, bool> IValidationConstraint<T>.Validate => ValidateProperty;

		private bool ValidateProperty(T model)
		{
			var value = PropertyValue(model);
			if (value != null)
			{
				if (Double.TryParse(value.ToString(), out double d))
				{
					return d > 0;
				}
			}
			return true;
		}

		string IValidationConstraint<T>.Message => $"Property '{PropertyName}' cannot be empty.";
	}

	public class PositiveConstraint<T>(string propertyName, Func<T, object> propertyValue) : IValidationConstraint<T>
	{
		public string PropertyName { get; set; } = propertyName;
		public Func<T, object> PropertyValue { get; set; } = propertyValue;

		Func<T, bool> IValidationConstraint<T>.Validate => ValidateProperty;

		private bool ValidateProperty(T model)
		{
			var value = PropertyValue(model);
			if (value != null)
			{
				if (Double.TryParse(value.ToString(), out double d))
				{
					return d >= 0;
				}
			}
			return true;
		}

		string IValidationConstraint<T>.Message => $"Property '{PropertyName}' must be positive.";
	}

	public class NonZeroConstraint<T>(string propertyName, Func<T, object> propertyValue) : IValidationConstraint<T>
	{
		public string PropertyName { get; set; } = propertyName;
		public Func<T, object> PropertyValue { get; set; } = propertyValue;

		Func<T, bool> IValidationConstraint<T>.Validate => ValidateProperty;

		private bool ValidateProperty(T model)
		{
			var value = PropertyValue(model);
			if (value != null)
			{
				if (Double.TryParse(value.ToString(), out double d))
				{
					return d != 0;
				}
			}
			return true;
		}

		string IValidationConstraint<T>.Message => $"Property '{PropertyName}' cannot be zero.";
	}

	public class GreaterThanConstraint<T>(string propertyName, Func<T, object> propertyValue, double value) : IValidationConstraint<T>
	{
		public string PropertyName { get; set; } = propertyName;
		public Func<T, object> PropertyValue { get; set; } = propertyValue;
		public double Value { get; set; } = value;

		Func<T, bool> IValidationConstraint<T>.Validate => ValidateProperty;

		private bool ValidateProperty(T model)
		{
			var value = PropertyValue(model);
			if (value != null)
			{
				if (Double.TryParse(value.ToString(), out double d))
				{
					return d > Value;
				}
			}
			return true;
		}

		string IValidationConstraint<T>.Message => $"Property '{PropertyName}' must be greater than {Value}.";
	}

	public class NonGreaterThanConstraint<T> : IValidationConstraint<T>
	{
		public NonGreaterThanConstraint(string propertyName, Func<T, object> propertyValue, double value, string valueDesc = null)
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

		Func<T, bool> IValidationConstraint<T>.Validate => ValidateProperty;

		private bool ValidateProperty(T model)
		{
			var value = PropertyValue(model);
			if (value != null)
			{
				if (Double.TryParse(value.ToString(), out double d))
				{
					return d <= Value;
				}
			}
			return true;
		}

		string IValidationConstraint<T>.Message => $"Property '{PropertyName}' cannot be greater than {ValueDesc}.";
	}

	public class LessThanConstraint<T>(string propertyName, Func<T, object> propertyValue, double value) : IValidationConstraint<T>
	{
		public string PropertyName { get; set; } = propertyName;
		public Func<T, object> PropertyValue { get; set; } = propertyValue;
		public double Value { get; set; } = value;

		Func<T, bool> IValidationConstraint<T>.Validate => ValidateProperty;

		private bool ValidateProperty(T model)
		{
			var value = PropertyValue(model);
			if (value != null)
			{
				if (Double.TryParse(value.ToString(), out double d))
				{
					return d < Value;
				}
			}
			return true;
		}

		string IValidationConstraint<T>.Message => $"Property '{PropertyName}' must be less than {Value}.";
	}

	public class EmailValidationConstraint<T>(string propertyName, Func<T, string> propertyValue) : IValidationConstraint<T>
	{
		public string PropertyName { get; set; } = propertyName;
		private readonly Func<T, string> _propertyValue = propertyValue;
		private readonly string _pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

		public Func<T, bool> Validate => ValidateEmail;

		private bool ValidateEmail(T model)
		{
			var email = _propertyValue(model);
			if (string.IsNullOrEmpty(email))
			{
				return false;
			}
			return System.Text.RegularExpressions.Regex.IsMatch(email, _pattern);
		}

		public string Message => $"Property '{PropertyName}' is not a valid email address.";
	}
}
