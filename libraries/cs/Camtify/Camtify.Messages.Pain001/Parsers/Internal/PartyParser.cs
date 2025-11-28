using System.Globalization;
using System.Xml;
using Camtify.Domain.Common;
using Camtify.Messages.Pain001.Models.Pain001;

namespace Camtify.Messages.Pain001.Parsers.Internal;

/// <summary>
/// Internal parser for party-related ISO 20022 structures.
/// </summary>
/// <remarks>
/// Handles parsing of PartyIdentification, PostalAddress, PartyId, and related structures.
/// </remarks>
internal sealed class PartyParser
{
    /// <summary>
    /// Parses PartyIdentification from XmlReader subtree.
    /// </summary>
    internal async Task<PartyIdentification> ParsePartyIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        string? name = null;
        PostalAddress? postalAddress = null;
        PartyId? identification = null;
        string? countryOfResidence = null;

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "InitgPty" || subtree.LocalName == "Dbtr" || subtree.LocalName == "UltmtDbtr" || subtree.LocalName == "Cdtr" || subtree.LocalName == "UltmtCdtr")
            {
                continue;
            }

            switch (subtree.LocalName)
            {
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
                case "PstlAdr":
                    postalAddress = await ParsePostalAddressAsync(subtree, ns, cancellationToken);
                    break;
                case "Id":
                    identification = await ParsePartyIdAsync(subtree, ns, cancellationToken);
                    break;
                case "CtryOfRes":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            countryOfResidence = await subtree.GetValueAsync();
                        }
                    }
                    break;
            }
        }

        return new PartyIdentification
        {
            Name = name,
            PostalAddress = postalAddress,
            Identification = identification,
            CountryOfResidence = countryOfResidence
        };
    }

    /// <summary>
    /// Parses PostalAddress from XmlReader subtree.
    /// </summary>
    internal async Task<PostalAddress> ParsePostalAddressAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        string? country = null;
        var addressLines = new List<string>();

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "PstlAdr")
            {
                continue;
            }

            switch (subtree.LocalName)
            {
                case "Ctry":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            country = await subtree.GetValueAsync();
                        }
                    }
                    break;
                case "AdrLine":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            var line = await subtree.GetValueAsync();
                            addressLines.Add(line);
                        }
                    }
                    break;
            }
        }

        return new PostalAddress
        {
            Country = country,
            AddressLines = addressLines.Count > 0 ? addressLines : null
        };
    }

    /// <summary>
    /// Parses PartyId from XmlReader subtree.
    /// </summary>
    internal async Task<PartyId?> ParsePartyIdAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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
                case "OrgId":
                    return await ParseOrganisationPartyIdAsync(subtree, ns, cancellationToken);
                case "PrvtId":
                    return await ParsePersonPartyIdAsync(subtree, ns, cancellationToken);
            }
        }

        return null;
    }

    /// <summary>
    /// Parses OrganisationPartyId from XmlReader subtree.
    /// </summary>
    internal async Task<OrganisationPartyId> ParseOrganisationPartyIdAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        string? anyBic = null;
        string? lei = null;
        var otherIdentifications = new List<GenericIdentification>();

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "OrgId")
            {
                continue;
            }

            switch (subtree.LocalName)
            {
                case "AnyBIC":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            anyBic = await subtree.GetValueAsync();
                        }
                    }
                    break;
                case "LEI":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            lei = await subtree.GetValueAsync();
                        }
                    }
                    break;
                case "Othr":
                    var other = await ParseGenericIdentificationAsync(subtree, ns, cancellationToken);
                    otherIdentifications.Add(other);
                    break;
            }
        }

        return new OrganisationPartyId
        {
            Organisation = new OrganisationIdentification
            {
                AnyBic = anyBic,
                Lei = lei,
                Other = otherIdentifications.Count > 0 ? otherIdentifications : null
            }
        };
    }

    /// <summary>
    /// Parses PersonPartyId from XmlReader subtree.
    /// </summary>
    internal async Task<PersonPartyId> ParsePersonPartyIdAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        DateAndPlaceOfBirth? dateAndPlaceOfBirth = null;
        var otherIdentifications = new List<GenericPersonIdentification>();

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "PrvtId")
            {
                continue;
            }

            switch (subtree.LocalName)
            {
                case "DtAndPlcOfBirth":
                    dateAndPlaceOfBirth = await ParseDateAndPlaceOfBirthAsync(subtree, ns, cancellationToken);
                    break;
                case "Othr":
                    var other = await ParseGenericPersonIdentificationAsync(subtree, ns, cancellationToken);
                    otherIdentifications.Add(other);
                    break;
            }
        }

        return new PersonPartyId
        {
            Person = new PersonIdentification
            {
                DateAndPlaceOfBirth = dateAndPlaceOfBirth,
                Other = otherIdentifications.Count > 0 ? otherIdentifications : null
            }
        };
    }

    /// <summary>
    /// Parses DateAndPlaceOfBirth from XmlReader subtree.
    /// </summary>
    internal async Task<DateAndPlaceOfBirth> ParseDateAndPlaceOfBirthAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        DateOnly birthDate = default;
        string? cityOfBirth = null;
        string? countryOfBirth = null;

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "DtAndPlcOfBirth")
            {
                continue;
            }

            switch (subtree.LocalName)
            {
                case "BirthDt":
                    string? birthDtStr = null;
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            birthDtStr = await subtree.GetValueAsync();
                        }
                    }
                    if (birthDtStr != null)
                    {
                        birthDate = DateOnly.Parse(birthDtStr, CultureInfo.InvariantCulture);
                    }
                    break;
                case "CityOfBirth":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            cityOfBirth = await subtree.GetValueAsync();
                        }
                    }
                    break;
                case "CtryOfBirth":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            countryOfBirth = await subtree.GetValueAsync();
                        }
                    }
                    break;
            }
        }

        return new DateAndPlaceOfBirth
        {
            BirthDate = birthDate,
            CityOfBirth = cityOfBirth ?? throw new ArgumentException("Missing CityOfBirth in DtAndPlcOfBirth."),
            CountryOfBirth = countryOfBirth ?? throw new ArgumentException("Missing CtryOfBirth in DtAndPlcOfBirth.")
        };
    }

    /// <summary>
    /// Parses GenericIdentification from XmlReader subtree.
    /// </summary>
    internal async Task<GenericIdentification> ParseGenericIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        string? id = null;
        string? schemeName = null;
        string? issuer = null;

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

            switch (subtree.LocalName)
            {
                case "Id":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            id = await subtree.GetValueAsync();
                        }
                    }
                    break;
                case "SchmeNm":
                    schemeName = await ParseSchemeNameAsync(subtree, ns, cancellationToken);
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

        return new GenericIdentification
        {
            Id = id ?? throw new ArgumentException("Missing Id in Othr."),
            SchemeName = schemeName,
            Issuer = issuer
        };
    }

    /// <summary>
    /// Parses GenericPersonIdentification from XmlReader subtree.
    /// </summary>
    internal async Task<GenericPersonIdentification> ParseGenericPersonIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        string? id = null;
        PersonIdentificationSchemeName? schemeName = null;

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

            switch (subtree.LocalName)
            {
                case "Id":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            id = await subtree.GetValueAsync();
                        }
                    }
                    break;
                case "SchmeNm":
                    schemeName = await ParsePersonIdentificationSchemeNameAsync(subtree, ns, cancellationToken);
                    break;
            }
        }

        return new GenericPersonIdentification
        {
            Identification = id ?? throw new ArgumentException("Missing Id in Othr."),
            SchemeName = schemeName
        };
    }

    /// <summary>
    /// Parses SchemeName from XmlReader subtree.
    /// </summary>
    internal async Task<string?> ParseSchemeNameAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "SchmeNm")
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
                            return await subtree.GetValueAsync();
                        }
                    }
                    break;
                case "Prtry":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            return await subtree.GetValueAsync();
                        }
                    }
                    break;
            }
        }

        return null;
    }

    /// <summary>
    /// Parses PersonIdentificationSchemeName from XmlReader subtree.
    /// </summary>
    internal async Task<PersonIdentificationSchemeName?> ParsePersonIdentificationSchemeNameAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        string? code = null;
        string? proprietary = null;

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "SchmeNm")
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
                            code = await subtree.GetValueAsync();
                        }
                    }
                    break;
                case "Prtry":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            proprietary = await subtree.GetValueAsync();
                        }
                    }
                    break;
            }
        }

        if (code == null && proprietary == null)
        {
            return null;
        }

        return new PersonIdentificationSchemeName
        {
            Code = code,
            Proprietary = proprietary
        };
    }
}
