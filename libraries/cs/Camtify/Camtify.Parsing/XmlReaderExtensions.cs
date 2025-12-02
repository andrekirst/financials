using System.Globalization;
using System.Text;
using System.Xml;
using Camtify.Core.Parsing;

namespace Camtify.Parsing;

/// <summary>
/// Extension methods for XmlReader operations.
/// </summary>
public static class XmlReaderExtensions
{
    /// <summary>
    /// Creates a secure XmlReader for a file.
    /// </summary>
    /// <param name="factory">The XmlReader factory</param>
    /// <param name="filePath">Path to the XML file</param>
    /// <returns>Configured XmlReader</returns>
    public static XmlReader CreateSecureReader(this IXmlReaderFactory factory, string filePath)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 8192,
            useAsync: true);

        return factory.Create(stream);
    }

    /// <summary>
    /// Creates a secure XmlReader for a string.
    /// </summary>
    /// <param name="factory">The XmlReader factory</param>
    /// <param name="xml">XML content as string</param>
    /// <returns>Configured XmlReader</returns>
    public static XmlReader CreateFromString(this IXmlReaderFactory factory, string xml)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentException.ThrowIfNullOrWhiteSpace(xml);

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        return factory.Create(stream);
    }

    #region Content Reading Methods

    /// <summary>
    /// Reads element content as string, handling empty elements.
    /// </summary>
    /// <param name="reader">The XML reader.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The element content, or null if empty.</returns>
    public static async Task<string?> ReadElementContentAsStringOrDefaultAsync(
        this XmlReader reader,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader);

        if (reader.IsEmptyElement)
        {
            await reader.ReadAsync();
            return null;
        }

        return await reader.ReadElementContentAsStringAsync();
    }

    /// <summary>
    /// Reads element content as decimal.
    /// </summary>
    /// <param name="reader">The XML reader.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed decimal, or null if empty or invalid.</returns>
    public static async Task<decimal?> ReadElementContentAsDecimalAsync(
        this XmlReader reader,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var content = await reader.ReadElementContentAsStringOrDefaultAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        if (decimal.TryParse(content, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        return null;
    }

    /// <summary>
    /// Reads element content as DateTime.
    /// </summary>
    /// <param name="reader">The XML reader.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed DateTime, or null if empty or invalid.</returns>
    public static async Task<DateTime?> ReadElementContentAsDateTimeAsync(
        this XmlReader reader,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var content = await reader.ReadElementContentAsStringOrDefaultAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        if (DateTime.TryParse(content, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var result))
        {
            return result;
        }

        return null;
    }

    /// <summary>
    /// Reads element content as int.
    /// </summary>
    /// <param name="reader">The XML reader.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed int, or null if empty or invalid.</returns>
    public static async Task<int?> ReadElementContentAsIntAsync(
        this XmlReader reader,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var content = await reader.ReadElementContentAsStringOrDefaultAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        if (int.TryParse(content, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        return null;
    }

    /// <summary>
    /// Reads element content as bool.
    /// </summary>
    /// <param name="reader">The XML reader.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed bool, or null if empty or invalid.</returns>
    public static async Task<bool?> ReadElementContentAsBoolAsync(
        this XmlReader reader,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var content = await reader.ReadElementContentAsStringOrDefaultAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        if (bool.TryParse(content, out var result))
        {
            return result;
        }

        return null;
    }

    #endregion

    #region Navigation Methods

    /// <summary>
    /// Moves to the next element with the specified name.
    /// </summary>
    /// <param name="reader">The XML reader.</param>
    /// <param name="elementName">The element name to find.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if found, false otherwise.</returns>
    public static async Task<bool> MoveToElementAsync(
        this XmlReader reader,
        string elementName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentException.ThrowIfNullOrWhiteSpace(elementName);

        while (await reader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == elementName)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Reads to the first child element.
    /// </summary>
    /// <param name="reader">The XML reader.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a child element was found, false otherwise.</returns>
    public static async Task<bool> ReadToFirstChildElementAsync(
        this XmlReader reader,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader);

        if (reader.IsEmptyElement)
        {
            return false;
        }

        while (await reader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (reader.NodeType == XmlNodeType.Element)
            {
                return true;
            }

            if (reader.NodeType == XmlNodeType.EndElement)
            {
                return false;
            }
        }

        return false;
    }

    /// <summary>
    /// Reads to the next sibling element with the specified name.
    /// </summary>
    /// <param name="reader">The XML reader.</param>
    /// <param name="elementName">The element name to find.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if found, false otherwise.</returns>
    public static async Task<bool> ReadToSiblingAsync(
        this XmlReader reader,
        string elementName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentException.ThrowIfNullOrWhiteSpace(elementName);

        var depth = reader.Depth;

        while (await reader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (reader.Depth < depth)
            {
                return false;
            }

            if (reader.Depth == depth && reader.NodeType == XmlNodeType.Element && reader.LocalName == elementName)
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Complex Structure Parsing

    /// <summary>
    /// Reads a date element that may contain nested Dt or DtTm elements.
    /// </summary>
    /// <param name="reader">The XML reader positioned at date element (e.g., BookgDt, ValDt).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed DateTime, or null if not found.</returns>
    /// <remarks>
    /// Handles ISO 20022 date patterns:
    /// - BookgDt/Dt or BookgDt/DtTm
    /// - ValDt/Dt or ValDt/DtTm
    /// </remarks>
    public static async Task<DateTime?> ReadDateElementAsync(
        this XmlReader reader,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader);

        if (reader.IsEmptyElement)
        {
            await reader.ReadAsync();
            return null;
        }

        var startDepth = reader.Depth;
        await reader.ReadAsync(); // Enter date element

        while (reader.Depth > startDepth)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.LocalName == "Dt" || reader.LocalName == "DtTm")
                {
                    return await reader.ReadElementContentAsDateTimeAsync(cancellationToken);
                }
            }

            await reader.ReadAsync();
        }

        return null;
    }

    /// <summary>
    /// Reads an amount element with currency attribute.
    /// </summary>
    /// <param name="reader">The XML reader positioned at amount element (e.g., Amt).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple of amount and currency code.</returns>
    /// <remarks>
    /// Parses elements like: &lt;Amt Ccy="EUR"&gt;123.45&lt;/Amt&gt;
    /// </remarks>
    public static async Task<(decimal? Amount, string? Currency)> ReadAmountWithCurrencyAsync(
        this XmlReader reader,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader);

        string? currency = reader.GetAttribute("Ccy");
        decimal? amount = await reader.ReadElementContentAsDecimalAsync(cancellationToken);

        return (amount, currency);
    }

    /// <summary>
    /// Skips the current element and all its children.
    /// </summary>
    /// <param name="reader">The XML reader.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SkipElementAsync(
        this XmlReader reader,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader);

        if (reader.NodeType != XmlNodeType.Element)
        {
            return;
        }

        await reader.SkipAsync();
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Checks if the reader is currently positioned at an element with the specified name.
    /// </summary>
    /// <param name="reader">The XML reader.</param>
    /// <param name="elementName">The element name to check.</param>
    /// <returns>True if at the specified element, false otherwise.</returns>
    public static bool IsAtElement(this XmlReader reader, string elementName)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentException.ThrowIfNullOrWhiteSpace(elementName);

        return reader.NodeType == XmlNodeType.Element && reader.LocalName == elementName;
    }

    /// <summary>
    /// Reads an element if it exists at the current position.
    /// </summary>
    /// <param name="reader">The XML reader.</param>
    /// <param name="elementName">The element name to read.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The element content, or null if not found.</returns>
    public static async Task<string?> ReadElementIfPresentAsync(
        this XmlReader reader,
        string elementName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentException.ThrowIfNullOrWhiteSpace(elementName);

        if (reader.IsAtElement(elementName))
        {
            return await reader.ReadElementContentAsStringOrDefaultAsync(cancellationToken);
        }

        return null;
    }

    #endregion
}
