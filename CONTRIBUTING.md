# Contributing to Camtify

## Project Structure

Camtify is organized as a modular .NET solution with the following architecture:

### Core Projects

- **Camtify.Core** - Shared utilities, extensions, base classes
- **Camtify.Domain** - Domain models and entities shared across all business areas
- **Camtify.Parsing** - Parsing infrastructure and base parsers
- **Camtify.Validation** - Validation framework and validators
- **Camtify.Generation** - Code generation and file generation utilities
- **Camtify.Tests** - Unit and integration tests for all projects

### Business Area Projects

Each financial message standard or protocol has its own dedicated project:

- **Camtify.Camt** - ISO 20022 Cash Management Messages (camt.053, camt.054, etc.)
- **Camtify.Mt940** - SWIFT MT940 account statement format
- **Camtify.Pain** - ISO 20022 Payment Initiation Messages (pain.001, pain.008, etc.)
- _(Add more business areas as needed)_

## Business Area Project Template

When creating a new business area project, follow this structure:

```
Camtify.{BusinessArea}/
├── Camtify.{BusinessArea}.csproj
├── Models/
│   ├── {MessageType1}/
│   │   ├── {MessageType1}Document.cs
│   │   ├── {MessageType1}Header.cs
│   │   └── ...
│   └── {MessageType2}/
│       └── ...
├── Parsers/
│   ├── {MessageType1}Parser.cs
│   └── {MessageType2}Parser.cs
├── Validators/
│   ├── {MessageType1}Validator.cs
│   └── {MessageType2}Validator.cs
├── Generators/
│   ├── {MessageType1}Generator.cs
│   └── {MessageType2}Generator.cs
└── README.md
```

### Example: Camtify.Camt

```
Camtify.Camt/
├── Camtify.Camt.csproj
├── Models/
│   ├── Camt053/              # Bank to Customer Statement
│   │   ├── Camt053Document.cs
│   │   ├── GroupHeader.cs
│   │   ├── Statement.cs
│   │   └── Entry.cs
│   └── Camt054/              # Bank to Customer Debit/Credit Notification
│       └── ...
├── Parsers/
│   ├── Camt053Parser.cs
│   └── Camt054Parser.cs
├── Validators/
│   ├── Camt053Validator.cs
│   └── Camt054Validator.cs
├── Generators/
│   ├── Camt053Generator.cs
│   └── Camt054Generator.cs
└── README.md
```

## Naming Conventions

### Project Names
- Format: `Camtify.{BusinessArea}`
- Examples: `Camtify.Camt`, `Camtify.Pain`, `Camtify.Mt940`
- Use official abbreviations where available (e.g., CAMT, PAIN from ISO 20022)

### Namespace Structure
- Format: `Camtify.{BusinessArea}.{Feature}.{SubFeature}`
- Examples:
  - `Camtify.Camt.Models.Camt053`
  - `Camtify.Camt.Parsers`
  - `Camtify.Mt940.Validators`

### File Naming
- **PascalCase** for all file names
- Be descriptive and specific
- Examples:
  - `Camt053Document.cs` - Main document class
  - `Camt053Parser.cs` - Parser implementation
  - `Camt053Validator.cs` - Validator implementation
  - `GroupHeader.cs` - Nested model
  - `TransactionEntry.cs` - Nested model

### Class Naming
- Match file name (one public class per file)
- Interfaces: `I{Name}` (e.g., `IParser`, `IValidator`)
- Abstract classes: `{Name}Base` or `Abstract{Name}` (e.g., `ParserBase`)

## Folder Structure

### Required Folders
- **Models/** - Domain models, DTOs, entities for the business area
- **Parsers/** - Parser implementations for reading/deserializing formats
- **Validators/** - Validation logic for business rules and constraints
- **Generators/** - Code/file generation for serialization/export

### Optional Folders
- **Converters/** - Type converters, mappers between formats
- **Extensions/** - Extension methods specific to this business area
- **Schemas/** - XSD schemas, JSON schemas, or other specifications

## Project Configuration

### .csproj Settings

Each business area project should:

1. Use property variables from `Directory.Build.props`:

```xml
<PropertyGroup>
  <TargetFramework>$(ProjectTargetFramework)</TargetFramework>
  <LangVersion>$(ProjectLangVersion)</LangVersion>
  <Nullable>$(ProjectNullable)</Nullable>
  <ImplicitUsings>$(ProjectImplicitUsings)</ImplicitUsings>
  <!-- ... other shared properties ... -->

  <!-- Override for NuGet packaging -->
  <IsPackable>true</IsPackable>
  <PackageId>Camtify.{BusinessArea}</PackageId>
  <Description>Camtify support for {BusinessArea} financial messages</Description>
  <PackageTags>finance;banking;{businessarea};iso20022;swift</PackageTags>
</PropertyGroup>
```

2. Reference required core projects:

```xml
<ItemGroup>
  <ProjectReference Include="..\Camtify.Core\Camtify.Core.csproj" />
  <ProjectReference Include="..\Camtify.Domain\Camtify.Domain.csproj" />
  <ProjectReference Include="..\Camtify.Parsing\Camtify.Parsing.csproj" />
  <ProjectReference Include="..\Camtify.Validation\Camtify.Validation.csproj" />
  <ProjectReference Include="..\Camtify.Generation\Camtify.Generation.csproj" />
</ItemGroup>
```

### Dependency Rules

- Business area projects **MAY** depend on:
  - Camtify.Core
  - Camtify.Domain
  - Camtify.Parsing
  - Camtify.Validation
  - Camtify.Generation

- Business area projects **MUST NOT** depend on:
  - Other business area projects (keep them independent)
  - Camtify.Tests

- Shared code should go into Core, Domain, or appropriate infrastructure project

## NuGet Package Strategy

Each business area is published as a **separate NuGet package**:

- **Camtify.Core** - Base package with shared utilities
- **Camtify.Domain** - Domain models package
- **Camtify.Camt** - CAMT messages package (depends on Core + Domain)
- **Camtify.Mt940** - MT940 messages package (depends on Core + Domain)
- **Camtify.Pain** - PAIN messages package (depends on Core + Domain)

Benefits:
- Users install only what they need
- Independent versioning possible
- Smaller package sizes
- Clear separation of concerns

## Testing

### Test Organization

Tests are organized in `Camtify.Tests` with subfolders per business area:

```
Camtify.Tests/
├── Core/
│   └── ...
├── Domain/
│   └── ...
├── Camt/
│   ├── Camt053ParserTests.cs
│   ├── Camt053ValidatorTests.cs
│   └── Fixtures/
│       ├── camt053_example1.xml
│       └── camt053_example2.xml
├── Mt940/
│   ├── Mt940ParserTests.cs
│   └── Fixtures/
│       └── mt940_example1.txt
└── ...
```

### Test Naming
- Test classes: `{ClassUnderTest}Tests.cs`
- Test methods: `{MethodName}_{Scenario}_{ExpectedResult}`
- Example: `Parse_ValidCamt053Xml_ReturnsDocument()`

### Test Fixtures
- Store test files in `Camtify.Tests/{BusinessArea}/Fixtures/`
- Use descriptive names: `{format}_{scenario}.{ext}`
- Example: `camt053_valid_single_entry.xml`

## Documentation

### README.md per Business Area

Each business area project must include a README.md with:

1. **Overview** - What is this business area?
2. **Supported Message Types** - List of implemented message types
3. **Usage Examples** - Code examples for common scenarios
4. **Standards References** - Links to official specifications
5. **Installation** - NuGet package installation instructions

### XML Documentation Comments

- All public classes, methods, and properties must have XML documentation
- Use `<summary>`, `<param>`, `<returns>`, `<exception>` tags
- Example:

```csharp
/// <summary>
/// Parses ISO 20022 CAMT.053 XML documents into strongly-typed objects.
/// </summary>
public class Camt053Parser : IParser<Camt053Document>
{
    /// <summary>
    /// Parses the specified XML content into a CAMT.053 document.
    /// </summary>
    /// <param name="xmlContent">The XML content to parse.</param>
    /// <returns>A parsed CAMT.053 document.</returns>
    /// <exception cref="ParseException">Thrown when the XML is invalid or malformed.</exception>
    public Camt053Document Parse(string xmlContent)
    {
        // ...
    }
}
```

## Code Style

- Follow standard C# conventions
- Use nullable reference types (`<Nullable>enable</Nullable>`)
- Prefer explicit over implicit types for clarity
- Keep methods focused and single-purpose
- Use async/await for I/O operations

## Git Workflow

1. Create a feature branch: `feature/{businessarea}-{description}`
2. Make focused commits with clear messages
3. Reference issues in commit messages where applicable
4. Ensure all tests pass before committing
5. Create pull request for review

## Adding a New Business Area

1. Create project: `dotnet new classlib -n Camtify.{BusinessArea} -o libraries/cs/Camtify/Camtify.{BusinessArea}`
2. Update `.csproj` with property variables and `IsPackable=true`
3. Add project references to Core, Domain, Parsing, Validation, Generation
4. Create folder structure: Models, Parsers, Validators, Generators
5. Add project to `Camtify.sln`
6. Create business area README.md
7. Add test folder in `Camtify.Tests/{BusinessArea}/`
8. Update main README.md with new business area

## Questions?

Open an issue or discussion on GitHub for questions about contributing.
