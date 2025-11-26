using Camtify.Infrastructure.Extensions;

namespace Camtify.Domain.Common;

/// <summary>
/// Domain-specific extension methods for parsing ISO 20022 enum codes.
/// Provides business-logic for converting between ISO 20022 codes and domain enums.
/// </summary>
/// <remarks>
/// For generic enum utilities, see <see cref="Camtify.Infrastructure.Extensions.EnumExtensions"/>.
/// </remarks>
public static class Iso20022EnumExtensions
{
    /// <summary>
    /// Gets the ISO 20022 code from an enum value.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="enumValue">The enum value.</param>
    /// <returns>The ISO 20022 code string from the Description attribute.</returns>
    /// <example>
    /// <code>
    /// var status = TransactionStatus.Rejected;
    /// var code = status.ToIso20022Code(); // Returns "RJCT"
    /// </code>
    /// </example>
    public static string ToIso20022Code<TEnum>(this TEnum enumValue) where TEnum : struct, Enum
    {
        return enumValue.GetDescription();
    }

    /// <summary>
    /// Parses an ISO 20022 code string to a TransactionStatus enum value.
    /// </summary>
    /// <param name="code">The ISO 20022 code string (e.g., "RJCT", "ACSC").</param>
    /// <returns>The corresponding TransactionStatus enum value, or null if not found.</returns>
    /// <example>
    /// <code>
    /// var status = Iso20022EnumExtensions.ParseTransactionStatus("RJCT");
    /// // Returns TransactionStatus.Rejected
    /// </code>
    /// </example>
    public static TransactionStatus? ParseTransactionStatus(string? code)
    {
        return EnumExtensions.ParseFromDescription<TransactionStatus>(code);
    }

    /// <summary>
    /// Parses an ISO 20022 code string to a CreditDebitIndicator enum value.
    /// </summary>
    /// <param name="code">The ISO 20022 code string (e.g., "CRDT", "DBIT").</param>
    /// <returns>The corresponding CreditDebitIndicator enum value, or null if not found.</returns>
    public static CreditDebitIndicator? ParseCreditDebitIndicator(string? code)
    {
        return EnumExtensions.ParseFromDescription<CreditDebitIndicator>(code);
    }

    /// <summary>
    /// Parses an ISO 20022 code string to an EntryStatus enum value.
    /// </summary>
    /// <param name="code">The ISO 20022 code string (e.g., "BOOK", "PDNG").</param>
    /// <returns>The corresponding EntryStatus enum value, or null if not found.</returns>
    public static EntryStatus? ParseEntryStatus(string? code)
    {
        return EnumExtensions.ParseFromDescription<EntryStatus>(code);
    }
}
