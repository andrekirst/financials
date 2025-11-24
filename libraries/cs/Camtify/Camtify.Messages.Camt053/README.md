# Camtify.Camt

ISO 20022 Cash Management (CAMT) messages support for Camtify.

## Overview

This package provides parsing, validation, and generation capabilities for ISO 20022 CAMT messages, which are used for cash management and account reporting in banking systems.

## Supported Message Types

### Implemented
- **camt.053.001.02** - Bank to Customer Statement (Account statements with transaction details)
- **camt.054** - Bank to Customer Debit/Credit Notification (Incoming payment notifications)

### Planned
- **camt.052** - Bank to Customer Account Report (Intraday account information)
- **camt.060** - Account Reporting Request

## Installation

```bash
dotnet add package Camtify.Camt
```

## Usage Examples

### Parsing a CAMT.053 XML Document

```csharp
using Camtify.Camt.Parsers;
using Camtify.Camt.Models.Camt053;

var parser = new Camt053Parser();
var xmlContent = File.ReadAllText("account_statement.xml");

Camt053Document document = parser.Parse(xmlContent);

foreach (var statement in document.Statements)
{
    Console.WriteLine($"IBAN: {statement.Account.Iban}");
    Console.WriteLine($"Balance: {statement.Balance.Amount} {statement.Balance.Currency}");

    foreach (var entry in statement.Entries)
    {
        Console.WriteLine($"  {entry.BookingDate}: {entry.Amount} - {entry.RemittanceInfo}");
    }
}
```

### Validating a CAMT.053 Document

```csharp
using Camtify.Camt.Validators;

var validator = new Camt053Validator();
var validationResult = validator.Validate(document);

if (!validationResult.IsValid)
{
    foreach (var error in validationResult.Errors)
    {
        Console.WriteLine($"Error: {error.Message}");
    }
}
```

### Generating a CAMT.053 XML Document

```csharp
using Camtify.Camt.Generators;
using Camtify.Camt.Models.Camt053;

var document = new Camt053Document
{
    MessageId = "MSG-2025-11-24-001",
    CreationDateTime = DateTime.UtcNow,
    Statements = new List<Statement>
    {
        new Statement
        {
            Account = new Account { Iban = "DE89370400440532013000" },
            Balance = new Balance { Amount = 1000.00m, Currency = "EUR" },
            Entries = new List<Entry>
            {
                new Entry
                {
                    BookingDate = DateTime.Today,
                    Amount = 100.00m,
                    CreditDebitIndicator = CreditDebit.Credit,
                    RemittanceInfo = "Payment received"
                }
            }
        }
    }
};

var generator = new Camt053Generator();
string xml = generator.Generate(document);
File.WriteAllText("statement.xml", xml);
```

## Standards and References

- [ISO 20022 Official Website](https://www.iso20022.org/)
- [CAMT.053 Message Definition](https://www.iso20022.org/catalogue-messages/iso-20022-messages-archive?search=camt.053)
- [CAMT.054 Message Definition](https://www.iso20022.org/catalogue-messages/iso-20022-messages-archive?search=camt.054)
- [XSD Schemas](../../specs/camt.053.001.02.xsd)

## Contributing

See [CONTRIBUTING.md](../../CONTRIBUTING.md) for guidelines on adding new CAMT message types or improving existing implementations.

## License

This project is licensed under the MIT License - see the [LICENSE](../../LICENSE) file for details.
