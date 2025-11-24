# Camtify.Mt940

SWIFT MT940 account statement format support for Camtify.

## Overview

This package provides parsing, validation, and generation capabilities for SWIFT MT940 messages, a widely-used legacy format for electronic account statements in banking.

## What is MT940?

MT940 (Message Type 940) is a SWIFT standard for the electronic transmission of account statement data from banks to their corporate customers. Despite being a legacy format, it remains widely used alongside modern ISO 20022 standards.

## Features

- Parse MT940 text files into strongly-typed objects
- Validate MT940 messages against SWIFT specifications
- Generate MT940 formatted output from structured data
- Support for common field tags (:20:, :25:, :60F:, :61:, :62F:, :86:, etc.)

## Installation

```bash
dotnet add package Camtify.Mt940
```

## Usage Examples

### Parsing an MT940 File

```csharp
using Camtify.Mt940.Parsers;
using Camtify.Mt940.Models;

var parser = new Mt940Parser();
var mt940Content = File.ReadAllText("account_statement.mt940");

Mt940Document document = parser.Parse(mt940Content);

Console.WriteLine($"Transaction Reference: {document.TransactionReference}");
Console.WriteLine($"Account: {document.AccountIdentification}");
Console.WriteLine($"Opening Balance: {document.OpeningBalance.Amount} {document.OpeningBalance.Currency}");

foreach (var transaction in document.Transactions)
{
    Console.WriteLine($"{transaction.ValueDate}: {transaction.Amount} - {transaction.Description}");
}

Console.WriteLine($"Closing Balance: {document.ClosingBalance.Amount} {document.ClosingBalance.Currency}");
```

### Validating an MT940 Document

```csharp
using Camtify.Mt940.Validators;

var validator = new Mt940Validator();
var validationResult = validator.Validate(document);

if (!validationResult.IsValid)
{
    foreach (var error in validationResult.Errors)
    {
        Console.WriteLine($"Validation Error: {error.Message}");
    }
}
```

### Generating an MT940 File

```csharp
using Camtify.Mt940.Generators;
using Camtify.Mt940.Models;

var document = new Mt940Document
{
    TransactionReference = "REF123456",
    AccountIdentification = "DE89370400440532013000",
    StatementNumber = "001",
    OpeningBalance = new Balance
    {
        Amount = 1000.00m,
        Currency = "EUR",
        Date = new DateTime(2025, 11, 24),
        DebitCreditMark = "C"
    },
    Transactions = new List<Transaction>
    {
        new Transaction
        {
            ValueDate = new DateTime(2025, 11, 24),
            Amount = 100.00m,
            DebitCreditMark = "C",
            Description = "Payment received",
            Reference = "TRANSFER-001"
        }
    },
    ClosingBalance = new Balance
    {
        Amount = 1100.00m,
        Currency = "EUR",
        Date = new DateTime(2025, 11, 24),
        DebitCreditMark = "C"
    }
};

var generator = new Mt940Generator();
string mt940Text = generator.Generate(document);
File.WriteAllText("statement.mt940", mt940Text);
```

## MT940 Format Overview

### Main Field Tags

- **:20:** - Transaction Reference Number
- **:25:** - Account Identification
- **:28C:** - Statement Number/Sequence Number
- **:60F:** - Opening Balance
- **:61:** - Statement Line (Transaction)
- **:86:** - Information to Account Owner (Transaction Details)
- **:62F:** - Closing Balance
- **:64:** - Closing Available Balance (Optional)

### Example MT940 File

```
:20:REF123456
:25:DE89370400440532013000
:28C:001
:60F:C251124EUR1000,00
:61:251124C100,00NTRF TRANSFER-001
:86:Payment received
:62F:C251124EUR1100,00
```

## Standards and References

- [SWIFT MT940 Customer Statement Message](https://www.swift.com/standards/mt/mt-940)
- [MT940 Format Specification](https://www.swift.com/resource/mt940-format)
- [Test Fixtures](../../specs/test-fixtures/)

## Migration to ISO 20022

While MT940 remains widely used, many institutions are migrating to ISO 20022 CAMT messages. Consider using `Camtify.Camt` for modern implementations.

**MT940 → CAMT.053 Mapping:**
- MT940 Statement → CAMT.053 BankToCustomerStatementV02
- Both provide account statement information
- CAMT.053 offers richer structured data and better international support

## Contributing

See [CONTRIBUTING.md](../../CONTRIBUTING.md) for guidelines on improving MT940 support or adding new features.

## License

This project is licensed under the MIT License - see the [LICENSE](../../LICENSE) file for details.
