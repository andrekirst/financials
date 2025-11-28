using System.Xml;
using Camtify.Domain.Common;
using Camtify.Messages.Pain001.Models.Pain001;

namespace Camtify.Messages.Pain001.Parsers.Internal;

/// <summary>
/// Internal parser for financial institution-related ISO 20022 structures.
/// </summary>
/// <remarks>
/// Handles parsing of BranchAndFinancialInstitutionIdentification, FinancialInstitutionIdentification,
/// ClearingSystemMemberIdentification, and related structures.
/// </remarks>
internal sealed class FinancialInstitutionParser
{
    /// <summary>
    /// Parses BranchAndFinancialInstitutionIdentification from XmlReader subtree.
    /// </summary>
    internal async Task<BranchAndFinancialInstitutionIdentification> ParseBranchAndFinancialInstitutionIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        FinancialInstitutionIdentification? financialInstitutionIdentification = null;

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "FwdgAgt" || subtree.LocalName == "DbtrAgt" || subtree.LocalName == "IntrmyAgt1" || subtree.LocalName == "CdtrAgt")
            {
                continue;
            }

            if (subtree.LocalName == "FinInstnId")
            {
                financialInstitutionIdentification = await ParseFinancialInstitutionIdentificationAsync(subtree, ns, cancellationToken);
            }
        }

        return new BranchAndFinancialInstitutionIdentification
        {
            FinancialInstitutionIdentification = financialInstitutionIdentification ?? throw new ArgumentException("Missing FinInstnId.")
        };
    }

    /// <summary>
    /// Parses FinancialInstitutionIdentification from XmlReader subtree.
    /// </summary>
    internal async Task<FinancialInstitutionIdentification> ParseFinancialInstitutionIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        string? bic = null;
        string? name = null;
        ClearingSystemMemberIdentification? clearingSystemMemberId = null;

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "FinInstnId")
            {
                continue;
            }

            switch (subtree.LocalName)
            {
                case "BICFI":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            bic = await subtree.GetValueAsync();
                        }
                    }
                    break;
                case "Nm":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            name = await subtree.GetValueAsync();
                        }
                    }
                    break;
                case "ClrSysMmbId":
                    clearingSystemMemberId = await ParseClearingSystemMemberIdentificationAsync(subtree, ns, cancellationToken);
                    break;
            }
        }

        return new FinancialInstitutionIdentification
        {
            Bic = bic,
            Name = name,
            ClearingSystemMemberId = clearingSystemMemberId
        };
    }

    /// <summary>
    /// Parses ClearingSystemMemberIdentification from XmlReader subtree.
    /// </summary>
    internal async Task<ClearingSystemMemberIdentification> ParseClearingSystemMemberIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        string? clearingSystemId = null;
        string? memberId = null;

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "ClrSysMmbId")
            {
                continue;
            }

            switch (subtree.LocalName)
            {
                case "ClrSysId":
                    clearingSystemId = await ParseClearingSystemIdAsync(subtree, ns, cancellationToken);
                    break;
                case "MmbId":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            memberId = await subtree.GetValueAsync();
                        }
                    }
                    break;
            }
        }

        return new ClearingSystemMemberIdentification
        {
            ClearingSystemId = clearingSystemId,
            MemberId = memberId ?? throw new ArgumentException("Missing MmbId in ClrSysMmbId.")
        };
    }

    /// <summary>
    /// Parses clearing system ID from XmlReader subtree.
    /// </summary>
    internal async Task<string?> ParseClearingSystemIdAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "ClrSysId")
            {
                continue;
            }

            if (subtree.LocalName == "Cd")
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

        return null;
    }
}
