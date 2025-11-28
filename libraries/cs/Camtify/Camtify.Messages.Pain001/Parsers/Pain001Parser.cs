using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using Camtify.Core;
using Camtify.Domain.Common;
using Camtify.Messages.Pain001.Models.Pain001;

namespace Camtify.Messages.Pain001.Parsers;

/// <summary>
/// Streaming parser for pain.001 (Customer Credit Transfer Initiation) messages using XmlReader.
/// </summary>
/// <remarks>
/// Supports versions: 003, 008, 009, 010, 011.
/// Uses XmlReader for memory-efficient streaming of large documents.
/// </remarks>
public sealed class Pain001Parser : IStreamable<PaymentInformation>
{
    private static readonly Dictionary<string, string> SupportedVersions = new()
    {
        { "urn:iso:std:iso:20022:tech:xsd:pain.001.001.03", "003" },
        { "urn:iso:std:iso:20022:tech:xsd:pain.001.001.08", "008" },
        { "urn:iso:std:iso:20022:tech:xsd:pain.001.001.09", "009" },
        { "urn:iso:std:iso:20022:tech:xsd:pain.001.001.10", "010" },
        { "urn:iso:std:iso:20022:tech:xsd:pain.001.001.11", "011" },
    };

    private static readonly Dictionary<string, ChargeBearer> ChargeBearerMapping = new()
    {
        { "DEBT", ChargeBearer.Debtor },
        { "CRED", ChargeBearer.Creditor },
        { "SHAR", ChargeBearer.Shared },
        { "SLEV", ChargeBearer.FollowingServiceLevel }
    };

    private readonly Stream _stream;
    private readonly bool _leaveOpen;
    private readonly bool _cacheGroupHeader;
    private GroupHeader? _cachedGroupHeader;
    private string? _cachedNamespace;
    private string? _cachedVersion;

    /// <summary>
    /// Initializes a new instance of the <see cref="Pain001Parser"/> class.
    /// </summary>
    /// <param name="xmlStream">The XML stream to parse. Must be readable.</param>
    /// <param name="leaveOpen">If true, the stream will not be disposed after parsing.</param>
    /// <param name="cacheGroupHeader">If true, the GroupHeader will be cached during streaming.</param>
    public Pain001Parser(Stream xmlStream, bool leaveOpen = false, bool cacheGroupHeader = true)
    {
        ArgumentNullException.ThrowIfNull(xmlStream);

        if (!xmlStream.CanRead)
        {
            throw new ArgumentException("Stream must be readable.", nameof(xmlStream));
        }

        _stream = xmlStream;
        _leaveOpen = leaveOpen;
        _cacheGroupHeader = cacheGroupHeader;
    }

    /// <summary>
    /// Gets the cached GroupHeader if available.
    /// </summary>
    public GroupHeader? CachedGroupHeader => _cachedGroupHeader;

    /// <summary>
    /// Creates a Pain001Parser from a string.
    /// </summary>
    /// <param name="xml">The XML content as string.</param>
    /// <returns>A new Pain001Parser instance.</returns>
    public static Pain001Parser FromString(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            throw new ArgumentException("XML content cannot be null or empty.", nameof(xml));
        }

        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));
        return new Pain001Parser(stream, leaveOpen: false);
    }

    /// <summary>
    /// Creates a Pain001Parser from a file.
    /// </summary>
    /// <param name="filePath">The path to the XML file.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A new Pain001Parser instance.</returns>
    public static async Task<Pain001Parser> FromFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        return new Pain001Parser(fileStream, leaveOpen: false);
    }

    /// <summary>
    /// Parses the GroupHeader from the XML document.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed GroupHeader.</returns>
    /// <remarks>
    /// For seekable streams, this method can be called multiple times.
    /// For non-seekable streams, this will only work if called before any streaming operations,
    /// or if the GroupHeader was cached during streaming (cacheGroupHeader = true).
    /// </remarks>
    public async Task<GroupHeader> ParseGroupHeaderAsync(CancellationToken cancellationToken = default)
    {
        // Return cached GroupHeader if available
        if (_cachedGroupHeader is not null)
        {
            return _cachedGroupHeader;
        }

        // Reset stream to beginning if seekable
        if (_stream.CanSeek)
        {
            _stream.Position = 0;
        }

        using var reader = CreateXmlReader(closeInput: false);

        string? ns = null;

        while (await reader.ReadAsync())
        {
            if (reader.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Detect namespace on Document element
            if (reader.LocalName == "Document" && ns == null)
            {
                ns = reader.NamespaceURI;
                _cachedNamespace = ns;
                _cachedVersion = GetVersionFromNamespace(ns);
            }

            // Parse GrpHdr
            if (reader.LocalName == "GrpHdr" && ns != null)
            {
                var groupHeader = await ParseGroupHeaderFromReaderAsync(reader, ns, cancellationToken);

                if (_cacheGroupHeader)
                {
                    _cachedGroupHeader = groupHeader;
                }

                return groupHeader;
            }
        }

        throw new ArgumentException("GrpHdr element not found in XML document.");
    }

    /// <summary>
    /// Streams PaymentInformation entries from the XML document.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An async enumerable of PaymentInformation entries.</returns>
    public async IAsyncEnumerable<PaymentInformation> GetEntriesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var entry in GetPaymentInformationEntriesAsync(cancellationToken))
        {
            yield return entry;
        }
    }

    /// <summary>
    /// Streams PaymentInformation entries from the XML document.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An async enumerable of PaymentInformation entries.</returns>
    public async IAsyncEnumerable<PaymentInformation> GetPaymentInformationEntriesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Reset stream to beginning if seekable
        if (_stream.CanSeek)
        {
            _stream.Position = 0;
        }

        using var reader = CreateXmlReader(closeInput: false);

        string? ns = null;

        while (await reader.ReadAsync())
        {
            if (reader.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Detect namespace on Document element
            if (reader.LocalName == "Document" && ns == null)
            {
                ns = reader.NamespaceURI;
                _cachedNamespace = ns;
                _cachedVersion = GetVersionFromNamespace(ns);
            }

            // Cache GroupHeader if enabled
            if (reader.LocalName == "GrpHdr" && ns != null && _cacheGroupHeader && _cachedGroupHeader == null)
            {
                _cachedGroupHeader = await ParseGroupHeaderFromReaderAsync(reader, ns, cancellationToken);
                continue;
            }

            // Stream PaymentInformation
            if (reader.LocalName == "PmtInf" && ns != null)
            {
                yield return await ParsePaymentInformationFromReaderAsync(reader, ns, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Streams CreditTransferTransactionInformation entries directly from the XML document.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An async enumerable of CreditTransferTransactionInformation entries.</returns>
    public async IAsyncEnumerable<CreditTransferTransactionInformation> GetTransactionEntriesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Reset stream to beginning if seekable
        if (_stream.CanSeek)
        {
            _stream.Position = 0;
        }

        using var reader = CreateXmlReader(closeInput: false);

        string? ns = null;

        while (await reader.ReadAsync())
        {
            if (reader.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Detect namespace on Document element
            if (reader.LocalName == "Document" && ns == null)
            {
                ns = reader.NamespaceURI;
                _cachedNamespace = ns;
                _cachedVersion = GetVersionFromNamespace(ns);
            }

            // Cache GroupHeader if enabled
            if (reader.LocalName == "GrpHdr" && ns != null && _cacheGroupHeader && _cachedGroupHeader == null)
            {
                _cachedGroupHeader = await ParseGroupHeaderFromReaderAsync(reader, ns, cancellationToken);
                continue;
            }

            // Stream transactions directly
            if (reader.LocalName == "CdtTrfTxInf" && ns != null)
            {
                yield return await ParseCreditTransferTransactionInformationFromReaderAsync(reader, ns, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Parses a pain.001 XML document (backward compatibility).
    /// </summary>
    /// <param name="xml">The XML content as string.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed Pain001Document.</returns>
    public static async Task<Pain001Document> ParseAsync(string xml, CancellationToken cancellationToken = default)
    {
        var parser = FromString(xml);
        return await parser.ParseAsync(cancellationToken);
    }

    /// <summary>
    /// Parses a pain.001 XML document from a stream (backward compatibility).
    /// </summary>
    /// <param name="stream">The XML stream.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed Pain001Document.</returns>
    public static async Task<Pain001Document> ParseAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var parser = new Pain001Parser(stream, leaveOpen: true);
        return await parser.ParseAsync(cancellationToken);
    }

    /// <summary>
    /// Parses the entire document into a Pain001Document (non-streaming).
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed Pain001Document.</returns>
    private async Task<Pain001Document> ParseAsync(CancellationToken cancellationToken = default)
    {
        var groupHeader = await ParseGroupHeaderAsync(cancellationToken);

        // Reset stream for payment information parsing
        if (_stream.CanSeek)
        {
            _stream.Position = 0;
        }

        var paymentInformations = new List<PaymentInformation>();
        await foreach (var pmtInf in GetPaymentInformationEntriesAsync(cancellationToken))
        {
            paymentInformations.Add(pmtInf);
        }

        return new Pain001Document
        {
            CreditTransferInitiation = new CustomerCreditTransferInitiation
            {
                GroupHeader = groupHeader,
                PaymentInformation = paymentInformations
            },
            Version = _cachedVersion ?? throw new InvalidOperationException("Version not detected."),
            Namespace = _cachedNamespace ?? throw new InvalidOperationException("Namespace not detected.")
        };
    }

    private XmlReader CreateXmlReader(bool closeInput)
    {
        var settings = new XmlReaderSettings
        {
            Async = true,
            IgnoreWhitespace = true,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            CloseInput = closeInput,
            DtdProcessing = DtdProcessing.Prohibit,
            ValidationType = ValidationType.None
        };

        return XmlReader.Create(_stream, settings);
    }

    private static string GetVersionFromNamespace(string ns)
    {
        if (SupportedVersions.TryGetValue(ns, out var version))
        {
            return version;
        }

        throw new ArgumentException(
            $"Unsupported pain.001 version. Namespace: {ns}. Supported versions: {string.Join(", ", SupportedVersions.Values)}");
    }

    // XmlReader-based parser methods
    private async Task<GroupHeader> ParseGroupHeaderFromReaderAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        string? msgId = null;
        DateTimeOffset creDtTm = default;
        int nbOfTxs = 0;
        decimal? ctrlSum = null;
        PartyIdentification? initiatingParty = null;
        BranchAndFinancialInstitutionIdentification? forwardingAgent = null;

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "GrpHdr")
            {
                continue;
            }

            switch (subtree.LocalName)
            {
                case "MsgId":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            msgId = await subtree.GetValueAsync();
                        }
                    }
                    break;
                case "CreDtTm":
                    string? creDtTmStr = null;
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            creDtTmStr = await subtree.GetValueAsync();
                        }
                    }
                    if (creDtTmStr != null)
                    {
                        creDtTm = DateTimeOffset.Parse(creDtTmStr, CultureInfo.InvariantCulture);
                    }
                    break;
                case "NbOfTxs":
                    string? nbOfTxsStr = null;
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            nbOfTxsStr = await subtree.GetValueAsync();
                        }
                    }
                    if (nbOfTxsStr != null)
                    {
                        nbOfTxs = int.Parse(nbOfTxsStr, CultureInfo.InvariantCulture);
                    }
                    break;
                case "CtrlSum":
                    string? ctrlSumStr = null;
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            ctrlSumStr = await subtree.GetValueAsync();
                        }
                    }
                    if (ctrlSumStr != null)
                    {
                        ctrlSum = decimal.Parse(ctrlSumStr, CultureInfo.InvariantCulture);
                    }
                    break;
                case "InitgPty":
                    initiatingParty = await ParsePartyIdentificationAsync(subtree, ns, cancellationToken);
                    break;
                case "FwdgAgt":
                    forwardingAgent = await ParseBranchAndFinancialInstitutionIdentificationAsync(subtree, ns, cancellationToken);
                    break;
            }
        }

        return new GroupHeader
        {
            MessageIdentification = msgId ?? throw new ArgumentException("Missing MsgId in GrpHdr."),
            CreationDateTime = creDtTm,
            NumberOfTransactions = nbOfTxs,
            ControlSum = ctrlSum,
            InitiatingParty = initiatingParty ?? throw new ArgumentException("Missing InitgPty in GrpHdr."),
            ForwardingAgent = forwardingAgent
        };
    }

    private async Task<PaymentInformation> ParsePaymentInformationFromReaderAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        string? pmtInfId = null;
        string? pmtMtd = null;
        bool? btchBookg = null;
        int? nbOfTxs = null;
        decimal? ctrlSum = null;
        PaymentTypeInformation? paymentTypeInformation = null;
        DateOnly reqdExctnDt = default;
        PartyIdentification? debtor = null;
        CashAccount? debtorAccount = null;
        BranchAndFinancialInstitutionIdentification? debtorAgent = null;
        PartyIdentification? ultimateDebtor = null;
        ChargeBearer? chargeBearer = null;
        var creditTransferTransactionInformations = new List<CreditTransferTransactionInformation>();

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "PmtInf")
            {
                continue;
            }

            switch (subtree.LocalName)
            {
                case "PmtInfId":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            pmtInfId = await subtree.GetValueAsync();
                        }
                    }
                    break;
                case "PmtMtd":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            pmtMtd = await subtree.GetValueAsync();
                        }
                    }
                    break;
                case "BtchBookg":
                    string? btchBookgStr = null;
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            btchBookgStr = await subtree.GetValueAsync();
                        }
                    }
                    if (btchBookgStr != null)
                    {
                        btchBookg = bool.Parse(btchBookgStr);
                    }
                    break;
                case "NbOfTxs":
                    string? nbOfTxsStr = null;
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            nbOfTxsStr = await subtree.GetValueAsync();
                        }
                    }
                    if (nbOfTxsStr != null)
                    {
                        nbOfTxs = int.Parse(nbOfTxsStr, CultureInfo.InvariantCulture);
                    }
                    break;
                case "CtrlSum":
                    string? ctrlSumStr = null;
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            ctrlSumStr = await subtree.GetValueAsync();
                        }
                    }
                    if (ctrlSumStr != null)
                    {
                        ctrlSum = decimal.Parse(ctrlSumStr, CultureInfo.InvariantCulture);
                    }
                    break;
                case "PmtTpInf":
                    paymentTypeInformation = await ParsePaymentTypeInformationAsync(subtree, ns, cancellationToken);
                    break;
                case "ReqdExctnDt":
                    string? reqdExctnDtStr = null;
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            reqdExctnDtStr = await subtree.GetValueAsync();
                        }
                    }
                    if (reqdExctnDtStr != null)
                    {
                        reqdExctnDt = DateOnly.Parse(reqdExctnDtStr, CultureInfo.InvariantCulture);
                    }
                    break;
                case "Dbtr":
                    debtor = await ParsePartyIdentificationAsync(subtree, ns, cancellationToken);
                    break;
                case "DbtrAcct":
                    debtorAccount = await ParseCashAccountAsync(subtree, ns, cancellationToken);
                    break;
                case "DbtrAgt":
                    debtorAgent = await ParseBranchAndFinancialInstitutionIdentificationAsync(subtree, ns, cancellationToken);
                    break;
                case "UltmtDbtr":
                    ultimateDebtor = await ParsePartyIdentificationAsync(subtree, ns, cancellationToken);
                    break;
                case "ChrgBr":
                    string? chrgBrStr = null;
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            chrgBrStr = await subtree.GetValueAsync();
                        }
                    }
                    if (chrgBrStr != null && ChargeBearerMapping.TryGetValue(chrgBrStr, out var cb))
                    {
                        chargeBearer = cb;
                    }
                    break;
                case "CdtTrfTxInf":
                    var tx = await ParseCreditTransferTransactionInformationFromReaderAsync(subtree, ns, cancellationToken);
                    creditTransferTransactionInformations.Add(tx);
                    break;
            }
        }

        return new PaymentInformation
        {
            PaymentInformationIdentification = pmtInfId ?? throw new ArgumentException("Missing PmtInfId in PmtInf."),
            PaymentMethod = pmtMtd ?? throw new ArgumentException("Missing PmtMtd in PmtInf."),
            BatchBooking = btchBookg,
            NumberOfTransactions = nbOfTxs,
            ControlSum = ctrlSum,
            PaymentTypeInformation = paymentTypeInformation,
            RequestedExecutionDate = reqdExctnDt,
            Debtor = debtor ?? throw new ArgumentException("Missing Dbtr in PmtInf."),
            DebtorAccount = debtorAccount ?? throw new ArgumentException("Missing DbtrAcct in PmtInf."),
            DebtorAgent = debtorAgent ?? throw new ArgumentException("Missing DbtrAgt in PmtInf."),
            UltimateDebtor = ultimateDebtor,
            ChargeBearer = chargeBearer,
            CreditTransferTransactionInformation = creditTransferTransactionInformations
        };
    }

    private async Task<CreditTransferTransactionInformation> ParseCreditTransferTransactionInformationFromReaderAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        PaymentIdentification? paymentIdentification = null;
        PaymentTypeInformation? paymentTypeInformation = null;
        Money instructedAmount = default;
        ChargeBearer? chargeBearer = null;
        PartyIdentification? ultimateDebtor = null;
        BranchAndFinancialInstitutionIdentification? intermediaryAgent = null;
        BranchAndFinancialInstitutionIdentification? creditorAgent = null;
        PartyIdentification? creditor = null;
        CashAccount? creditorAccount = null;
        PartyIdentification? ultimateCreditor = null;
        string? purposeCode = null;
        RemittanceInformation? remittanceInformation = null;

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "CdtTrfTxInf")
            {
                continue;
            }

            switch (subtree.LocalName)
            {
                case "PmtId":
                    paymentIdentification = await ParsePaymentIdentificationAsync(subtree, ns, cancellationToken);
                    break;
                case "PmtTpInf":
                    paymentTypeInformation = await ParsePaymentTypeInformationAsync(subtree, ns, cancellationToken);
                    break;
                case "Amt":
                    instructedAmount = await ParseAmountAsync(subtree, ns, cancellationToken);
                    break;
                case "ChrgBr":
                    string? chrgBrStr = null;
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            chrgBrStr = await subtree.GetValueAsync();
                        }
                    }
                    if (chrgBrStr != null && ChargeBearerMapping.TryGetValue(chrgBrStr, out var cbTx))
                    {
                        chargeBearer = cbTx;
                    }
                    break;
                case "UltmtDbtr":
                    ultimateDebtor = await ParsePartyIdentificationAsync(subtree, ns, cancellationToken);
                    break;
                case "IntrmyAgt1":
                    intermediaryAgent = await ParseBranchAndFinancialInstitutionIdentificationAsync(subtree, ns, cancellationToken);
                    break;
                case "CdtrAgt":
                    creditorAgent = await ParseBranchAndFinancialInstitutionIdentificationAsync(subtree, ns, cancellationToken);
                    break;
                case "Cdtr":
                    creditor = await ParsePartyIdentificationAsync(subtree, ns, cancellationToken);
                    break;
                case "CdtrAcct":
                    creditorAccount = await ParseCashAccountAsync(subtree, ns, cancellationToken);
                    break;
                case "UltmtCdtr":
                    ultimateCreditor = await ParsePartyIdentificationAsync(subtree, ns, cancellationToken);
                    break;
                case "Purp":
                    purposeCode = await ParsePurposeCodeAsync(subtree, ns, cancellationToken);
                    break;
                case "RmtInf":
                    remittanceInformation = await ParseRemittanceInformationAsync(subtree, ns, cancellationToken);
                    break;
            }
        }

        return new CreditTransferTransactionInformation
        {
            PaymentIdentification = paymentIdentification ?? throw new ArgumentException("Missing PmtId in CdtTrfTxInf."),
            PaymentTypeInformation = paymentTypeInformation,
            InstructedAmount = instructedAmount,
            ChargeBearer = chargeBearer,
            UltimateDebtor = ultimateDebtor,
            IntermediaryAgent = intermediaryAgent,
            CreditorAgent = creditorAgent ?? throw new ArgumentException("Missing CdtrAgt in CdtTrfTxInf."),
            Creditor = creditor ?? throw new ArgumentException("Missing Cdtr in CdtTrfTxInf."),
            CreditorAccount = creditorAccount ?? throw new ArgumentException("Missing CdtrAcct in CdtTrfTxInf."),
            UltimateCreditor = ultimateCreditor,
            PurposeCode = purposeCode,
            RemittanceInformation = remittanceInformation
        };
    }

    private async Task<PartyIdentification> ParsePartyIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<PostalAddress> ParsePostalAddressAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<PartyId?> ParsePartyIdAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<OrganisationPartyId> ParseOrganisationPartyIdAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<PersonPartyId> ParsePersonPartyIdAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<DateAndPlaceOfBirth> ParseDateAndPlaceOfBirthAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<GenericIdentification> ParseGenericIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<GenericPersonIdentification> ParseGenericPersonIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<string?> ParseSchemeNameAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<PersonIdentificationSchemeName?> ParsePersonIdentificationSchemeNameAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<CashAccount> ParseCashAccountAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<AccountIdentification> ParseAccountIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<string> ParseOtherAccountIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<BranchAndFinancialInstitutionIdentification> ParseBranchAndFinancialInstitutionIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<FinancialInstitutionIdentification> ParseFinancialInstitutionIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<ClearingSystemMemberIdentification> ParseClearingSystemMemberIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<string?> ParseClearingSystemIdAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<PaymentTypeInformation> ParsePaymentTypeInformationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        string? instructionPriority = null;
        string? serviceLevelCode = null;
        string? localInstrumentCode = null;
        string? categoryPurposeCode = null;

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "PmtTpInf")
            {
                continue;
            }

            switch (subtree.LocalName)
            {
                case "InstrPrty":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            instructionPriority = await subtree.GetValueAsync();
                        }
                    }
                    break;
                case "SvcLvl":
                    serviceLevelCode = await ParseCodeAsync(subtree, ns, cancellationToken);
                    break;
                case "LclInstrm":
                    localInstrumentCode = await ParseCodeAsync(subtree, ns, cancellationToken);
                    break;
                case "CtgyPurp":
                    categoryPurposeCode = await ParseCodeAsync(subtree, ns, cancellationToken);
                    break;
            }
        }

        return new PaymentTypeInformation
        {
            InstructionPriority = instructionPriority,
            ServiceLevelCode = serviceLevelCode,
            LocalInstrumentCode = localInstrumentCode,
            CategoryPurposeCode = categoryPurposeCode
        };
    }

    private async Task<string?> ParseCodeAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself (could be SvcLvl, LclInstrm, or CtgyPurp)
            if (subtree.LocalName == "SvcLvl" || subtree.LocalName == "LclInstrm" || subtree.LocalName == "CtgyPurp")
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

    private async Task<PaymentIdentification> ParsePaymentIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        string? instrId = null;
        string? endToEndId = null;

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "PmtId")
            {
                continue;
            }

            switch (subtree.LocalName)
            {
                case "InstrId":
                    // Manually read the content without using ReadElementContentAsStringAsync
                    // to avoid advancing the reader past the next sibling
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            instrId = await subtree.GetValueAsync();
                        }
                    }
                    break;
                case "EndToEndId":
                    if (!subtree.IsEmptyElement)
                    {
                        await subtree.ReadAsync(); // Move to text node
                        if (subtree.NodeType == XmlNodeType.Text)
                        {
                            endToEndId = await subtree.GetValueAsync();
                        }
                    }
                    break;
            }
        }

        return new PaymentIdentification
        {
            InstructionIdentification = instrId,
            EndToEndIdentification = endToEndId ?? throw new ArgumentException($"Missing EndToEndId in PmtId. Found InstrId: {instrId}")
        };
    }

    private async Task<Money> ParseAmountAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "Amt")
            {
                continue;
            }

            if (subtree.LocalName == "InstdAmt")
            {
                var ccy = subtree.GetAttribute("Ccy");
                if (string.IsNullOrWhiteSpace(ccy))
                {
                    throw new ArgumentException("Missing Ccy attribute in InstdAmt.");
                }

                string? amountStr = null;
                if (!subtree.IsEmptyElement)
                {
                    await subtree.ReadAsync(); // Move to text node
                    if (subtree.NodeType == XmlNodeType.Text)
                    {
                        amountStr = await subtree.GetValueAsync();
                    }
                }

                if (amountStr != null)
                {
                    return Money.Parse(amountStr, ccy);
                }
            }
        }

        throw new ArgumentException("Missing InstdAmt in Amt.");
    }

    private async Task<string?> ParsePurposeCodeAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
    {
        using var subtree = reader.ReadSubtree();

        while (await subtree.ReadAsync())
        {
            if (subtree.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            // Skip the parent element itself
            if (subtree.LocalName == "Purp")
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

    private async Task<RemittanceInformation> ParseRemittanceInformationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<StructuredRemittanceInformation> ParseStructuredRemittanceInformationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<CreditorReferenceInformation> ParseCreditorReferenceInformationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<CreditorReferenceType?> ParseCreditorReferenceTypeAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    private async Task<CodeOrProprietary?> ParseCodeOrProprietaryAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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
