using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Hybrsoft.Enums
{
	public static class EnumExtensions
	{
		private static readonly ConcurrentDictionary<string, string> Cache = new();

		/// <summary>
		/// Retrieves the description associated with the specified enumeration value, supporting both standard and flag-based
		/// enumerations.
		/// </summary>
		/// <remarks>This method caches descriptions to improve performance on subsequent calls. It supports
		/// enumerations marked with the <see cref="FlagsAttribute"/> by returning descriptions for all set flags.</remarks>
		/// <typeparam name="TEnum">The type of the enumeration. Must be an enumeration type.</typeparam>
		/// <param name="value">The enumeration value for which to obtain the description. This parameter cannot be null.</param>
		/// <returns>A string containing the description of the specified enumeration value. For flag enumerations, returns a
		/// comma-separated list of descriptions for all active flags.</returns>
		/// <exception cref="ArgumentNullException">Thrown when the <paramref name="value"/> parameter is null.</exception>
		public static string GetDescription<TEnum>(this TEnum value)
			where TEnum : Enum
		{
			if (value is null)
				throw new ArgumentNullException(nameof(value));

			var type = value.GetType();
			var key = $"{type.FullName}.{value}";

			return Cache.GetOrAdd(key, _ =>
			{
				// Support for [Flags]
				if (type.IsDefined(typeof(FlagsAttribute), false))
				{
					var values = Enum.GetValues(type);

					var descriptions = new List<string>();

					foreach (Enum enumValue in values)
					{
						if (value.HasFlag(enumValue) && Convert.ToInt64(enumValue) != 0)
						{
							descriptions.Add(GetSingleValueDescription(enumValue));
						}
					}

					return descriptions.Count > 0
						? string.Join(", ", descriptions)
						: value.ToString();
				}

				// Enum normal
				return GetSingleValueDescription(value);
			});
		}

		/// <summary>
		/// Retrieves a user-friendly description for the specified enumeration value, prioritizing display attributes and
		/// descriptions if available.
		/// </summary>
		/// <remarks>This method checks for a <see cref="DisplayAttribute"/> first, followed by a <see
		/// cref="DescriptionAttribute"/>. If neither attribute is present, the enumeration's name is returned as a
		/// fallback.</remarks>
		/// <param name="value">The enumeration value for which to retrieve the description. This value must be a valid member of the enumeration
		/// type.</param>
		/// <returns>A string representing the description of the enumeration value. If no description is found, the enumeration name
		/// is returned.</returns>
		private static string GetSingleValueDescription(Enum value)
		{
			var name = value.ToString();
			var field = value.GetType().GetField(name);

			if (field == null)
				return name;

			// DisplayAttribute
			var display = field.GetCustomAttribute<DisplayAttribute>();
			if (!string.IsNullOrWhiteSpace(display?.Name))
				return display.Name;

			// DescriptionAttribute
			var description = field.GetCustomAttribute<DescriptionAttribute>();
			if (!string.IsNullOrWhiteSpace(description?.Description))
				return description.Description;

			// Fallback
			return name;
		}
	}
}