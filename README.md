# Camtify

A modular .NET library for parsing, validating, and generating financial messages and account statements.

## Overview

Camtify provides comprehensive support for various financial messaging standards and formats used in banking and finance, including:

- **ISO 20022** - Modern XML-based financial messaging standard
- **SWIFT MT Messages** - Legacy SWIFT message formats
- **Electronic Banking Formats** - Various account statement and payment formats

## Supported Standards

### ISO 20022 Messages

- **CAMT (Cash Management)** - `Camtify.Camt`
  - camt.053 - Bank to Customer Statement
  - camt.054 - Bank to Customer Debit/Credit Notification

### SWIFT Messages

- **MT940** - `Camtify.Mt940`
  - Account statement message format

### Coming Soon

- **PAIN (Payment Initiation)** - ISO 20022 payment messages
- **PACS (Payment Clearing)** - ISO 20022 payment clearing messages
- **MT942** - Interim Transaction Report

## Project Structure

```
Camtify/
├── Camtify.Core              # Shared utilities and base classes
├── Camtify.Domain            # Domain models and entities
├── Camtify.Parsing           # Parsing infrastructure
├── Camtify.Validation        # Validation framework
├── Camtify.Generation        # Code/file generation utilities
├── Camtify.Tests             # Test suite
│
├── Camtify.Camt              # ISO 20022 CAMT messages
├── Camtify.Mt940             # SWIFT MT940 format
└── ...                       # Additional business areas
```

## Installation

Install individual packages based on your needs:

```bash
# For ISO 20022 CAMT messages
dotnet add package Camtify.Camt

# For SWIFT MT940 messages
dotnet add package Camtify.Mt940

# For core functionality
dotnet add package Camtify.Core
dotnet add package Camtify.Domain
```

## Quick Start

### Parsing a CAMT.053 Statement

```csharp
using Camtify.Camt.Parsers;

var parser = new Camt053Parser();
var document = parser.Parse(File.ReadAllText("statement.xml"));

foreach (var statement in document.Statements)
{
    Console.WriteLine($"Account: {statement.Account.Iban}");
    Console.WriteLine($"Balance: {statement.Balance.Amount} {statement.Balance.Currency}");
}
```

### Parsing an MT940 Statement

```csharp
using Camtify.Mt940.Parsers;

var parser = new Mt940Parser();
var document = parser.Parse(File.ReadAllText("statement.mt940"));

Console.WriteLine($"Account: {document.AccountIdentification}");
Console.WriteLine($"Opening Balance: {document.OpeningBalance.Amount}");

foreach (var transaction in document.Transactions)
{
    Console.WriteLine($"{transaction.ValueDate}: {transaction.Amount}");
}
```

## Features

- **Parse** - Convert financial message formats (XML, text) into strongly-typed objects
- **Validate** - Validate messages against standards and business rules
- **Generate** - Create financial messages from structured data
- **Type-Safe** - Full C# type safety with nullable reference types
- **Extensible** - Modular architecture for adding new message types
- **Well-Documented** - Comprehensive XML documentation and examples

## Architecture

Camtify follows a modular architecture where:

1. **Core Projects** provide shared infrastructure
2. **Business Area Projects** implement specific message standards
3. Each business area is independently packageable as a NuGet package
4. Dependencies flow from business areas → infrastructure → core

See [CONTRIBUTING.md](CONTRIBUTING.md) for detailed architecture documentation.

## Development

### Requirements

- .NET 9.0 SDK or later
- Visual Studio 2022, VS Code, or Rider

### Building

```bash
cd libraries/cs/Camtify
dotnet build Camtify.sln
```

### Running Tests

```bash
dotnet test Camtify.sln
```

### Adding a New Business Area

See [CONTRIBUTING.md](CONTRIBUTING.md) for step-by-step instructions on adding new financial message standards.

## Documentation

- [Contributing Guidelines](CONTRIBUTING.md) - How to contribute and project conventions
- [Camtify.Camt Documentation](libraries/cs/Camtify/Camtify.Camt/README.md) - ISO 20022 CAMT messages
- [Camtify.Mt940 Documentation](libraries/cs/Camtify/Camtify.Mt940/README.md) - SWIFT MT940 format

## Standards References

- [ISO 20022 Official Website](https://www.iso20022.org/)
- [SWIFT Standards](https://www.swift.com/standards)
- [Specifications](specs/) - XSD schemas and test fixtures

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines on:

- Project structure and conventions
- Adding new business areas
- Coding standards
- Testing requirements
- Pull request process

## Roadmap

- [ ] Complete CAMT.053 parser implementation
- [ ] Complete MT940 parser implementation
- [ ] Add PAIN (Payment Initiation) support
- [ ] Add PACS (Payment Clearing) support
- [ ] Add MT942 support
- [ ] Implement XML Schema validation
- [ ] Add SEPA-specific validations
- [ ] Performance optimizations
- [ ] Extended documentation and examples

## Support

For questions, issues, or feature requests, please open an issue on GitHub.