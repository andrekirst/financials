# pain.001 Message Format Versions

This document provides a comprehensive overview of the supported pain.001 (Customer Credit Transfer Initiation) message format versions according to the ISO 20022 standard.

## Supported Versions

This library supports the following pain.001 versions:
- **v03** (pain.001.001.03) - SEPA baseline version
- **v08** (pain.001.001.08) - Instant payment support
- **v09** (pain.001.001.09) - **Recommended** for new implementations
- **v10** (pain.001.001.10) - Enhanced regulatory reporting
- **v11** (pain.001.001.11) - Latest version

## Quick Reference

### Feature Comparison

| Version | Status | LEI Support | Instant Payments | SEPA Baseline | Regulatory Reporting |
|---------|--------|-------------|------------------|---------------|---------------------|
| v03 | Legacy | ❌ | ❌ | ✅ | Basic |
| v08 | Active | ❌ | ✅ | ✅ | Enhanced |
| v09 | **Recommended** | ✅ | ✅ | ✅ | Enhanced |
| v10 | Active | ✅ | ✅ | ✅ | Advanced |
| v11 | Latest | ✅ | ✅ | ✅ | Advanced |

### Recommendations

📌 **Use v09 for new implementations** - Most widely adopted version with LEI support and full SEPA compliance.

⚠️ **SEPA Migration Deadline: November 2026** - All SEPA systems using v03 or v08 must migrate to v09 or higher.

### Namespace URIs

| Version | Namespace URI |
|---------|--------------|
| v03 | `urn:iso:std:iso:20022:tech:xsd:pain.001.001.03` |
| v08 | `urn:iso:std:iso:20022:tech:xsd:pain.001.001.08` |
| v09 | `urn:iso:std:iso:20022:tech:xsd:pain.001.001.09` |
| v10 | `urn:iso:std:iso:20022:tech:xsd:pain.001.001.10` |
| v11 | `urn:iso:std:iso:20022:tech:xsd:pain.001.001.11` |

## Version Details

### Version 003 (pain.001.001.03)

**Status**: Legacy (SEPA baseline)

**Introduced**: Early SEPA implementation

**Business Context**: Foundational structure for SEPA credit transfers.

**Key Features**:
- Basic party identification
- Standard remittance information
- SEPA-compliant payment structure

**When to Use**:
- ⚠️ **Migrate to v09+ by November 2026** (SEPA requirement)
- Legacy system compatibility only

**Technical Notes**: No LEI support, simpler postal address structure.

---

### Version 008 (pain.001.001.08)

**Status**: Active (instant payment support)

**Introduced**: ~2015

**Business Context**: Added support for instant payments (SEPA Instant Credit Transfer).

**Key Features**:
- Instant payment indicator (TISS)
- Enhanced service level codes
- Extended payment type information

**When to Use**:
- Systems requiring instant payment support without LEI
- ⚠️ **Migrate to v09+ by November 2026**

**Technical Notes**: No LEI support, instant payment elements added.

---

### Version 009 (pain.001.001.09) ⭐

**Status**: **Recommended** (most commonly used)

**Introduced**: 2019

**Business Context**: Industry standard for modern SEPA implementations.

**Key Features**:
- **Legal Entity Identifier (LEI) support** (regulatory requirement)
- Enhanced regulatory reporting
- Structured cross-border data
- Additional postal address fields
- RF creditor reference support

**When to Use**:
- ✅ **All new implementations** (industry standard)
- LEI reporting requirements
- Cross-border SEPA payments
- SEPA compliance (November 2026 deadline)

**Technical Notes**: Adds `<LEI>` element in organization identification, backward-compatible XML structure.

---

### Version 010 (pain.001.001.10)

**Status**: Active (incremental update)

**Introduced**: ~2020

**Business Context**: Enhanced validation and reporting on top of v09.

**Key Features**:
- All v09 features
- Enhanced validation rules
- Additional regulatory reporting fields
- Extended code value sets

**When to Use**:
- Systems requiring latest regulatory fields
- Future-proofing beyond v09

**Technical Notes**: Backward-compatible with v09, additive changes only.

---

### Version 011 (pain.001.001.11)

**Status**: Latest (cutting edge)

**Introduced**: 2021+

**Business Context**: Latest ISO 20022 enhancements.

**Key Features**:
- All v10 features
- Latest code value additions
- Enhanced schema validation

**When to Use**:
- Early adopters requiring latest standard
- Systems with specific v11-only requirements

**Technical Notes**: Backward-compatible with v09/v10, additive changes only.

## Technical Differences

### Main Structural Difference: LEI Support (v09+)

The most significant structural change between older versions (v03, v08) and newer versions (v09, v10, v11) is the addition of Legal Entity Identifier (LEI) support.

**v03/v08 Structure** (without LEI):
```xml
<InitgPty>
  <Nm>Acme Corporation</Nm>
  <PstlAdr>
    <Ctry>DE</Ctry>
    <AdrLine>Musterstrasse 123</AdrLine>
  </PstlAdr>
</InitgPty>
```

**v09/v10/v11 Structure** (with LEI):
```xml
<InitgPty>
  <Nm>Acme Corporation</Nm>
  <PstlAdr>
    <Ctry>DE</Ctry>
    <AdrLine>Musterstrasse 123</AdrLine>
  </PstlAdr>
  <Id>
    <OrgId>
      <LEI>529900T8BM49AURSDO55</LEI>
    </OrgId>
  </Id>
</InitgPty>
```

### Model Compatibility

**Key Technical Notes**:
- All versions share the same C# domain models (`CustomerCreditTransferInitiation`, `GroupHeader`, `PaymentInformation`, etc.)
- Version detection is automatic via XML namespace
- Parser base class handles 95%+ of parsing logic (version-agnostic)
- LEI element is optional in v09+ (backward-compatible)
- No code changes required when switching between versions

## Migration Guide

### SEPA Compliance Timeline

⚠️ **Important Deadline**: November 2026

**Requirement**: All SEPA payment systems must migrate to pain.001.001.09 or higher.

**Affected Systems**: Currently using v03 or v08.

### Upgrade Path

| From | To | Effort | Breaking Changes | Key Actions |
|------|----|----|-----------------|-------------|
| v03 → v09 | Recommended | Low | None (additive) | Update namespace, add optional LEI fields |
| v08 → v09 | Recommended | Low | None (additive) | Update namespace, add optional LEI fields |
| v09 → v10 | Optional | Minimal | None (additive) | Update namespace if new fields needed |
| v09 → v11 | Optional | Minimal | None (additive) | Update namespace if new fields needed |

### Migration Steps

1. **Update XML namespace URI** in your message generation code
2. **Add LEI fields** where available (optional but recommended for regulatory compliance)
3. **Test with parser factory** - automatic version detection handles all versions transparently
4. **Validate against new schema** (if using XSD validation)

### Risk Mitigation

- ✅ All versions use additive changes (backward-compatible XML structure)
- ✅ Parser handles all versions transparently via automatic detection
- ✅ No C# code changes required (same domain models across all versions)
- ✅ LEI fields are optional in v09+ (gradual adoption possible)

## Parser Usage Examples

### Automatic Version Detection (Recommended)

The factory automatically detects the version from the XML namespace and instantiates the appropriate parser:

```csharp
using Camtify.Messages.Pain001.Parsers;

// Factory automatically detects version from XML namespace
var parser = await Pain001ParserFactory.CreateAsync(stream);
var document = await parser.ParseDocumentAsync();

// Version information is accessible
Console.WriteLine($"Detected version: {parser.Version}");
Console.WriteLine($"Namespace: {parser.Namespace}");
```

### Version-Specific Instantiation (Performance Optimization)

If you know the version ahead of time, you can instantiate the parser directly:

```csharp
using Camtify.Messages.Pain001.Parsers;

// Direct instantiation for known version
var parser = new Pain001v09Parser(stream);
var document = await parser.ParseDocumentAsync();

// Or from string/file
var parser = Pain001v09Parser.FromString(xmlString);
var parser = await Pain001v09Parser.FromFileAsync("payment.xml");
```

## References

### ISO 20022 Standards

- [ISO 20022 Official Site](https://www.iso20022.org/)
- [pain.001 Message Definition](https://www.iso20022.org/payments_messages.page)

### SEPA Guidelines

- [EPC SEPA Credit Transfer Rulebook](https://www.europeanpaymentscouncil.eu/document-library/rulebooks/sepa-credit-transfer-rulebook)
- [SEPA Instant Credit Transfer Scheme Rulebook](https://www.europeanpaymentscouncil.eu/document-library/rulebooks/sepa-instant-credit-transfer-rulebook)

### Internal Documentation

- **Parser implementation**: `Camtify.Messages.Pain001/Parsers/`
- **Test data examples**: `Camtify.Messages.Pain001.Tests/TestData/`
- **Domain models**: `Camtify.Messages.Pain001/Models/Common/`

---

## Summary by Audience

### For Developers
- Use `Pain001ParserFactory.CreateAsync()` for automatic version detection
- All versions share the same domain models (no code changes needed)
- LEI support is the main structural difference (v09+)

### For Business Users
- **v09 is the industry standard** (most widely adopted)
- **November 2026 SEPA deadline** requires v09+ for compliance
- LEI support enables regulatory reporting (v09+)

### For Architects
- Low-effort upgrade path (additive changes, no breaking changes)
- v09 provides optimal balance of features and stability
- v10/v11 for future-proofing, but v09 sufficient for most use cases
