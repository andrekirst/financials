using System.Xml;
using Camtify.Domain.Common;
using Camtify.Messages.Pain001.Models.Pain001;

namespace Camtify.Messages.Pain001.Parsers.Internal;

/// <summary>
/// Internal parser for remittance information-related ISO 20022 structures.
/// </summary>
/// <remarks>
/// Handles parsing of RemittanceInformation, StructuredRemittanceInformation,
/// CreditorReferenceInformation, and related structures.
/// </remarks>
internal sealed class RemittanceParser
{
    /// <summary>
    /// Parses RemittanceInformation from XmlReader subtree.
    /// </summary>
    internal async Task<RemittanceInformation> ParseRemittanceInformationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        var unstructured = new List<string>();
        var structured = new List<StructuredRemittanceInformation>();

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "RmtInf")
            {
                continue;
            }

            switch (subtree.LocalName)
            {
                case "Ustrd":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            var ustrdStr = await subtree.GetValueAsync();
                            unstructured.Add(ustrdStr);
                        }
                    }
                    break;
                case "Strd":
                    var strd = await ParseStructuredRemittanceInformationAsync(subtree, ns, cancellationToken);
                    structured.Add(strd);
                    break;
            }
        }

        return new RemittanceInformation(
            unstructured: unstructured.Count > 0 ? unstructured : null,
            structured: structured.Count > 0 ? structured : null);
    }

    /// <summary>
    /// Parses StructuredRemittanceInformation from XmlReader subtree.
    /// </summary>
    internal async Task<StructuredRemittanceInformation> ParseStructuredRemittanceInformationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        CreditorReferenceInformation? creditorReferenceInformation = null;

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "Strd")
            {
                continue;
            }

            if (subtree.LocalName == "CdtrRefInf")
            {
                creditorReferenceInformation = await ParseCreditorReferenceInformationAsync(subtree, ns, cancellationToken);
            }
        }

        return new StructuredRemittanceInformation(
            creditorReferenceInformation: creditorReferenceInformation,
            referredDocumentInformation: null,
            additionalRemittanceInformation: null);
    }

    /// <summary>
    /// Parses CreditorReferenceInformation from XmlReader subtree.
    /// </summary>
    internal async Task<CreditorReferenceInformation> ParseCreditorReferenceInformationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        CreditorReferenceType? creditorReferenceType = null;
        string? reference = null;

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "CdtrRefInf")
            {
                continue;
            }

            switch (subtree.LocalName)
            {
                case "Tp":
                    creditorReferenceType = await ParseCreditorReferenceTypeAsync(subtree, ns, cancellationToken);
                    break;
                case "Ref":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            reference = await subtree.GetValueAsync();
                        }
                    }
                    break;
            }
        }

        return new CreditorReferenceInformation(creditorReferenceType, reference);
    }

    /// <summary>
    /// Parses CreditorReferenceType from XmlReader subtree.
    /// </summary>
    internal async Task<CreditorReferenceType?> ParseCreditorReferenceTypeAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        CodeOrProprietary? codeOrProprietary = null;
        string? issuer = null;

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "Tp")
            {
                continue;
            }

            switch (subtree.LocalName)
            {
                case "CdOrPrtry":
                    codeOrProprietary = await ParseCodeOrProprietaryAsync(subtree, ns, cancellationToken);
                    break;
                case "Issr":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            issuer = await subtree.GetValueAsync();
                        }
                    }
                    break;
            }
        }

        if (codeOrProprietary == null)
        {
            return null;
        }

        return new CreditorReferenceType(codeOrProprietary, issuer);
    }

    /// <summary>
    /// Parses CodeOrProprietary from XmlReader subtree.
    /// </summary>
    internal async Task<CodeOrProprietary?> ParseCodeOrProprietaryAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "CdOrPrtry")
            {
                continue;
            }

            switch (subtree.LocalName)
            {
                case "Cd":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            var code = await subtree.GetValueAsync();
                            return CodeOrProprietary.FromCode(code);
                        }
                    }
                    break;
                case "Prtry":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            var proprietary = await subtree.GetValueAsync();
                            return CodeOrProprietary.FromProprietary(proprietary);
                        }
                    }
                    break;
            }
        }

        return null;
    }
}
