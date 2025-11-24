# Contributing to Camtify

Thank you for your interest in contributing to Camtify! This document provides guidelines and instructions for contributing.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Pull Request Process](#pull-request-process)
- [Coding Standards](#coding-standards)
- [Testing Guidelines](#testing-guidelines)

## Code of Conduct

This project adheres to a code of conduct. By participating, you are expected to uphold this code.

## Getting Started

1. **Fork the repository**
   ```bash
   git clone https://github.com/andrekirst/financials.git
   cd financials/libraries/cs/Camtify
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run tests**
   ```bash
   dotnet test
   ```

## Development Workflow

1. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make your changes**
   - Follow the coding standards (see below)
   - Add tests for new functionality
   - Update documentation as needed

3. **Run tests locally**
   ```bash
   dotnet test --collect:"XPlat Code Coverage"
   ```

4. **Check formatting**
   ```bash
   dotnet format
   ```

5. **Commit your changes**
   ```bash
   git commit -m "feat: add new feature"
   ```

   Use conventional commits:
   - `feat:` - New feature
   - `fix:` - Bug fix
   - `docs:` - Documentation changes
   - `test:` - Test changes
   - `refactor:` - Code refactoring
   - `chore:` - Maintenance tasks

6. **Push to your fork**
   ```bash
   git push origin feature/your-feature-name
   ```

7. **Open a Pull Request**

## Pull Request Process

1. Ensure all tests pass
2. Update the README.md with details of changes if applicable
3. The PR must receive approval from at least one maintainer
4. All CI checks must pass:
   - Build succeeds
   - All tests pass
   - Code coverage meets threshold
   - SonarCloud quality gate passes
   - CodeQL security checks pass

## Coding Standards

### General Guidelines

- Follow .NET naming conventions
- Use meaningful variable and method names
- Keep methods small and focused
- Add XML documentation comments for public APIs
- Use nullable reference types consistently

### Example

```csharp
/// <summary>
/// Parses a CAMT.053 XML document.
/// </summary>
/// <param name="xml">The XML content to parse.</param>
/// <returns>A parsed CAMT.053 document.</returns>
/// <exception cref="ArgumentNullException">Thrown when xml is null.</exception>
public Camt053Document Parse(string xml)
{
    ArgumentNullException.ThrowIfNull(xml);
    // Implementation
}
```

## Testing Guidelines

### Test Structure

- One test class per production class
- Use descriptive test method names
- Follow AAA pattern (Arrange, Act, Assert)

### Example

```csharp
public class Camt053ParserTests
{
    [Fact]
    public void Parse_ValidXml_ReturnsParsedDocument()
    {
        // Arrange
        var parser = new Camt053Parser();
        var xml = LoadTestXml("valid-camt053.xml");

        // Act
        var result = parser.Parse(xml);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("CAMT.053.001.02", result.MessageType);
    }

    [Fact]
    public void Parse_InvalidXml_ThrowsException()
    {
        // Arrange
        var parser = new Camt053Parser();
        var xml = "<invalid>";

        // Act & Assert
        Assert.Throws<ParsingException>(() => parser.Parse(xml));
    }
}
```

### Test Coverage

- Aim for at least 80% code coverage
- All public APIs must have tests
- Test edge cases and error conditions
- Use test fixtures for complex setup

## Adding a New Business Area

See [CONTRIBUTING.md](../libraries/cs/Camtify/CONTRIBUTING.md) in the solution directory for detailed instructions on adding new business areas.

## Questions?

Feel free to open an issue for any questions or concerns!

## License

By contributing, you agree that your contributions will be licensed under the MIT License.
