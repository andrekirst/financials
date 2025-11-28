using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using Camtify.Core;
using Camtify.Domain.Common;
using Camtify.Messages.Pain001.Models.Pain001;
using Camtify.Messages.Pain001.Parsers.Internal;

namespace Camtify.Messages.Pain001.Parsers;

/// <summary>
/// Abstract base class for version-specific pain.001 parsers.
/// </summary>
/// <typeparam name="TDocument">The version-specific document type.</typeparam>
/// <remarks>
/// Contains all common parsing logic shared across pain.001 versions.
/// Derived classes provide version-specific document creation and namespace handling.
/// </remarks>
public abstract class Pain001ParserBase<TDocument> : IStreamable<PaymentInformation>
    where TDocument : Core.IIso20022Document<CustomerCreditTransferInitiation>
{
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

    // Lazy-initialized specialized parsers for reduced coupling
    private readonly Lazy<PartyParser> _partyParser = new(() => new PartyParser());
    private readonly Lazy<FinancialInstitutionParser> _financialInstitutionParser = new(() => new FinancialInstitutionParser());
    private readonly Lazy<AccountParser> _accountParser = new(() => new AccountParser());
    private readonly Lazy<RemittanceParser> _remittanceParser = new(() => new RemittanceParser());

    /// <summary>
    /// Initializes a new instance of the parser base class.
    /// </summary>
    /// <param name="xmlStream">The XML stream to parse. Must be readable.</param>
    /// <param name="leaveOpen">If true, the stream will not be disposed after parsing.</param>
    /// <param name="cacheGroupHeader">If true, the GroupHeader will be cached during streaming.</param>
    protected Pain001ParserBase(Stream xmlStream, bool leaveOpen = false, bool cacheGroupHeader = true)
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
    /// Gets the expected XML namespace URI for this parser version.
    /// </summary>
    protected abstract string GetExpectedNamespace();

    /// <summary>
    /// Gets the version identifier for this parser (e.g., "003", "009").
    /// </summary>
    protected abstract string GetVersion();

    /// <summary>
    /// Parses the entire document into a version-specific document instance.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed document.</returns>
    public abstract Task<TDocument> ParseDocumentAsync(CancellationToken cancellationToken = default);

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
                ValidateNamespace(ns);
                _cachedNamespace = ns;
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
                ValidateNamespace(ns);
                _cachedNamespace = ns;
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
                ValidateNamespace(ns);
                _cachedNamespace = ns;
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
    /// Creates an XmlReader with appropriate settings for parsing.
    /// </summary>
    /// <param name="closeInput">If true, the input stream will be closed when the reader is disposed.</param>
    /// <returns>A configured XmlReader instance.</returns>
    protected XmlReader CreateXmlReader(bool closeInput)
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

    /// <summary>
    /// Validates that the namespace matches the expected namespace for this parser version.
    /// </summary>
    /// <param name="ns">The namespace to validate.</param>
    /// <exception cref="ArgumentException">Thrown if namespace doesn't match expected.</exception>
    protected void ValidateNamespace(string ns)
    {
        var expectedNs = GetExpectedNamespace();
        if (ns != expectedNs)
        {
            throw new ArgumentException(
                $"Namespace mismatch. Expected: {expectedNs}, Found: {ns}. " +
                $"Use the correct parser for this pain.001 version.");
        }
    }

    // Protected parse methods - all available for derived classes to use or override

    /// <summary>
    /// Parses GroupHeader from XmlReader subtree.
    /// </summary>
    protected virtual async Task<GroupHeader> ParseGroupHeaderFromReaderAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    /// <summary>
    /// Parses PaymentInformation from XmlReader subtree.
    /// </summary>
    protected virtual async Task<PaymentInformation> ParsePaymentInformationFromReaderAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    /// <summary>
    /// Parses CreditTransferTransactionInformation from XmlReader subtree.
    /// </summary>
    protected virtual async Task<CreditTransferTransactionInformation> ParseCreditTransferTransactionInformationFromReaderAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    /// <summary>
    /// Parses PartyIdentification from XmlReader subtree.
    /// </summary>
    private async Task<PartyIdentification> ParsePartyIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _partyParser.Value.ParsePartyIdentificationAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses PostalAddress from XmlReader subtree.
    /// </summary>
    private async Task<PostalAddress> ParsePostalAddressAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _partyParser.Value.ParsePostalAddressAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses PartyId from XmlReader subtree.
    /// </summary>
    private async Task<PartyId?> ParsePartyIdAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _partyParser.Value.ParsePartyIdAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses OrganisationPartyId from XmlReader subtree.
    /// </summary>
    private async Task<OrganisationPartyId> ParseOrganisationPartyIdAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _partyParser.Value.ParseOrganisationPartyIdAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses PersonPartyId from XmlReader subtree.
    /// </summary>
    private async Task<PersonPartyId> ParsePersonPartyIdAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _partyParser.Value.ParsePersonPartyIdAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses DateAndPlaceOfBirth from XmlReader subtree.
    /// </summary>
    private async Task<DateAndPlaceOfBirth> ParseDateAndPlaceOfBirthAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _partyParser.Value.ParseDateAndPlaceOfBirthAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses GenericIdentification from XmlReader subtree.
    /// </summary>
    private async Task<GenericIdentification> ParseGenericIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _partyParser.Value.ParseGenericIdentificationAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses GenericPersonIdentification from XmlReader subtree.
    /// </summary>
    private async Task<GenericPersonIdentification> ParseGenericPersonIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _partyParser.Value.ParseGenericPersonIdentificationAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses SchemeName from XmlReader subtree.
    /// </summary>
    private async Task<string?> ParseSchemeNameAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _partyParser.Value.ParseSchemeNameAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses PersonIdentificationSchemeName from XmlReader subtree.
    /// </summary>
    private async Task<PersonIdentificationSchemeName?> ParsePersonIdentificationSchemeNameAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _partyParser.Value.ParsePersonIdentificationSchemeNameAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses CashAccount from XmlReader subtree.
    /// </summary>
    private async Task<CashAccount> ParseCashAccountAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _accountParser.Value.ParseCashAccountAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses AccountIdentification from XmlReader subtree.
    /// </summary>
    private async Task<AccountIdentification> ParseAccountIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _accountParser.Value.ParseAccountIdentificationAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses other account identification from XmlReader subtree.
    /// </summary>
    private async Task<string> ParseOtherAccountIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _accountParser.Value.ParseOtherAccountIdentificationAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses BranchAndFinancialInstitutionIdentification from XmlReader subtree.
    /// </summary>
    private async Task<BranchAndFinancialInstitutionIdentification> ParseBranchAndFinancialInstitutionIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _financialInstitutionParser.Value.ParseBranchAndFinancialInstitutionIdentificationAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses FinancialInstitutionIdentification from XmlReader subtree.
    /// </summary>
    private async Task<FinancialInstitutionIdentification> ParseFinancialInstitutionIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _financialInstitutionParser.Value.ParseFinancialInstitutionIdentificationAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses ClearingSystemMemberIdentification from XmlReader subtree.
    /// </summary>
    private async Task<ClearingSystemMemberIdentification> ParseClearingSystemMemberIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _financialInstitutionParser.Value.ParseClearingSystemMemberIdentificationAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses clearing system ID from XmlReader subtree.
    /// </summary>
    private async Task<string?> ParseClearingSystemIdAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _financialInstitutionParser.Value.ParseClearingSystemIdAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses PaymentTypeInformation from XmlReader subtree.
    /// </summary>
    protected virtual async Task<PaymentTypeInformation> ParsePaymentTypeInformationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    /// <summary>
    /// Parses code from XmlReader subtree.
    /// </summary>
    protected virtual async Task<string?> ParseCodeAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    /// <summary>
    /// Parses PaymentIdentification from XmlReader subtree.
    /// </summary>
    protected virtual async Task<PaymentIdentification> ParsePaymentIdentificationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    /// <summary>
    /// Parses Money amount from XmlReader subtree.
    /// </summary>
    protected virtual async Task<Money> ParseAmountAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    /// <summary>
    /// Parses purpose code from XmlReader subtree.
    /// </summary>
    protected virtual async Task<string?> ParsePurposeCodeAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
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

    /// <summary>
    /// Parses RemittanceInformation from XmlReader subtree.
    /// </summary>
    private async Task<RemittanceInformation> ParseRemittanceInformationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _remittanceParser.Value.ParseRemittanceInformationAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses StructuredRemittanceInformation from XmlReader subtree.
    /// </summary>
    private async Task<StructuredRemittanceInformation> ParseStructuredRemittanceInformationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _remittanceParser.Value.ParseStructuredRemittanceInformationAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses CreditorReferenceInformation from XmlReader subtree.
    /// </summary>
    private async Task<CreditorReferenceInformation> ParseCreditorReferenceInformationAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _remittanceParser.Value.ParseCreditorReferenceInformationAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses CreditorReferenceType from XmlReader subtree.
    /// </summary>
    private async Task<CreditorReferenceType?> ParseCreditorReferenceTypeAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _remittanceParser.Value.ParseCreditorReferenceTypeAsync(reader, ns, cancellationToken);

    /// <summary>
    /// Parses CodeOrProprietary from XmlReader subtree.
    /// </summary>
    private async Task<CodeOrProprietary?> ParseCodeOrProprietaryAsync(XmlReader reader, string ns, CancellationToken cancellationToken)
        => await _remittanceParser.Value.ParseCodeOrProprietaryAsync(reader, ns, cancellationToken);
}
