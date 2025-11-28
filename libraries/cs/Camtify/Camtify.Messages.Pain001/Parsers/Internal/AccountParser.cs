using System.Xml;
using Camtify.Domain.Common;
using Camtify.Messages.Pain001.Models.Pain001;

namespace Camtify.Messages.Pain001.Parsers.Internal;

/// <summary>
/// Internal parser for account-related ISO 20022 structures.
/// </summary>
/// <remarks>
/// Handles parsing of CashAccount, AccountIdentification, and related structures.
/// </remarks>
internal sealed class AccountParser
{
    /// <summary>
    /// Parses CashAccount from XmlReader subtree.
    /// </summary>
    internal async Task<CashAccount> ParseCashAccountAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        AccountIdentification? accountIdentification = null;
        CurrencyCode? currency = null;

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "DbtrAcct" || subtree.LocalName == "CdtrAcct")
            {
                continue;
            }

            switch (subtree.LocalName)
            {
                case "Id":
                    accountIdentification = await ParseAccountIdentificationAsync(subtree, ns, cancellationToken);
                    break;
                case "Ccy":
                    string? ccyStr = null;
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            ccyStr = await subtree.GetValueAsync();
                        }
                    }
                    if (ccyStr != null)
                    {
                        currency = CurrencyCode.Parse(ccyStr);
                    }
                    break;
            }
        }

        return new CashAccount
        {
            Identification = accountIdentification ?? throw new ArgumentException("Missing Id in account."),
            Currency = currency
        };
    }

    /// <summary>
    /// Parses AccountIdentification from XmlReader subtree.
    /// </summary>
    internal async Task<AccountIdentification> ParseAccountIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "Id")
            {
                continue;
            }

            switch (subtree.LocalName)
            {
                case "IBAN":
                    string? ibanStr = null;
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            ibanStr = await subtree.GetValueAsync();
                        }
                    }
                    if (ibanStr != null)
                    {
                        return new IbanAccountIdentification
                        {
                            Iban = Iban.Parse(ibanStr)
                        };
                    }
                    break;
                case "Othr":
                    var id = await ParseOtherAccountIdentificationAsync(subtree, ns, cancellationToken);
                    return new OtherAccountIdentification
                    {
                        Other = new GenericAccountIdentification
                        {
                            Identification = id
                        }
                    };
            }
        }

        throw new ArgumentException("No valid account identification found.");
    }

    /// <summary>
    /// Parses other account identification from XmlReader subtree.
    /// </summary>
    internal async Task<string> ParseOtherAccountIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "Othr")
            {
                continue;
            }

            if (subtree.LocalName == "Id")
            {
                if (!subtree.IsEmptyElement)
                {
                    await subtree.ReadAsync(); // Move to text node
                    if (subtree.NodeType == XmlNodeType.Text)
                    {
                        return await subtree.GetValueAsync();
                    }
                }
            }
        }

        throw new ArgumentException("Missing Id in Othr account identification.");
    }
}
