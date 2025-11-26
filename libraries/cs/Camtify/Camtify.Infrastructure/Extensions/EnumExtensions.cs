using System.ComponentModel;
using System.Reflection;

namespace Camtify.Infrastructure.Extensions;

/// <summary>
/// Generic extension methods for enum types.
/// Provides technical infrastructure for working with enums and their Description attributes.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets the Description attribute value from an enum value.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="enumValue">The enum value.</param>
    /// <returns>The description, or the enum name if no description attribute exists.</returns>
    /// <example>
    /// <code>
    /// var status = TransactionStatus.Rejected;
    /// var code = status.GetDescription(); // Returns "RJCT"
    /// </code>
    /// </example>
    public static string GetDescription<TEnum>(this TEnum enumValue) where TEnum : struct, Enum
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        if (fieldInfo == null)
        {
            return enumValue.ToString();
        }

        var attribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? enumValue.ToString();
    }

    /// <summary>
    /// Parses a string to an enum value by matching the Description attribute.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="description">The description string to match.</param>
    /// <returns>The corresponding enum value, or null if not found.</returns>
    /// <example>
    /// <code>
    /// var status = EnumExtensions.ParseFromDescription&lt;TransactionStatus&gt;("RJCT");
    /// // Returns TransactionStatus.Rejected
    /// </code>
    /// </example>
    public static TEnum? ParseFromDescription<TEnum>(string? description) where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return null;
        }

        var type = typeof(TEnum);

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();

            if (attribute != null && attribute.Description.Equals(description, StringComparison.OrdinalIgnoreCase))
            {
                return (TEnum?)field.GetValue(null);
            }
        }

        return null;
    }

    /// <summary>
    /// Tries to parse a string to an enum value by matching the Description attribute.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="description">The description string to match.</param>
    /// <param name="result">The parsed enum value if successful.</param>
    /// <returns>True if parsing succeeded, false otherwise.</returns>
    public static bool TryParseFromDescription<TEnum>(string? description, out TEnum result) where TEnum : struct, Enum
    {
        var parsed = ParseFromDescription<TEnum>(description);

        if (parsed.HasValue)
        {
            result = parsed.Value;
            return true;
        }

        result = default;
        return false;
    }
}
