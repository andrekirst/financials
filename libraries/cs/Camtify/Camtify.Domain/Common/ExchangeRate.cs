namespace Camtify.Domain.Common;

/// <summary>
/// Exchange rate between two currencies (XML: XchgRate in ForeignExchangeDetails).
/// </summary>
/// <remarks>
/// Represents the conversion rate from source currency to target currency.
/// 1 SourceCurrency = Rate * TargetCurrency
/// </remarks>
public sealed record ExchangeRate
{
    /// <summary>
    /// Gets the source currency (XML: SrcCcy).
    /// </summary>
    public required CurrencyCode SourceCurrency { get; init; }

    /// <summary>
    /// Gets the target currency (XML: TrgtCcy).
    /// </summary>
    public required CurrencyCode TargetCurrency { get; init; }

    /// <summary>
    /// Gets the exchange rate (XML: XchgRate).
    /// </summary>
    /// <remarks>
    /// 1 SourceCurrency = Rate * TargetCurrency
    /// </remarks>
    public required decimal Rate { get; init; }

    /// <summary>
    /// Gets the rate type (XML: RateTp).
    /// </summary>
    public ExchangeRateType? RateType { get; init; }

    /// <summary>
    /// Gets the quotation date (XML: CtrctId.QtnDt).
    /// </summary>
    public DateTime? QuotationDate { get; init; }

    /// <summary>
    /// Gets the contract identification for forward contracts (XML: CtrctId.Id).
    /// </summary>
    public string? ContractIdentification { get; init; }

    /// <summary>
    /// Converts a Money amount from source currency to target currency.
    /// </summary>
    /// <param name="source">The source amount to convert.</param>
    /// <returns>The converted amount in target currency.</returns>
    /// <exception cref="InvalidOperationException">If source currency does not match.</exception>
    public Money Convert(Money source)
    {
        if (source.Currency != SourceCurrency)
        {
            throw new InvalidOperationException(
                $"Can only convert {SourceCurrency}, not {source.Currency}.");
        }

        return new Money(source.Amount * Rate, TargetCurrency);
    }

    /// <summary>
    /// Creates an inverted exchange rate (swap source and target).
    /// </summary>
    /// <returns>The inverted exchange rate.</returns>
    public ExchangeRate Invert() => new()
    {
        SourceCurrency = TargetCurrency,
        TargetCurrency = SourceCurrency,
        Rate = 1m / Rate,
        RateType = RateType,
        QuotationDate = QuotationDate,
        ContractIdentification = ContractIdentification
    };
}
