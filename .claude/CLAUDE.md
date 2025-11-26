# Claude Code - Projekt Richtlinien

Dieses Dokument enthält projektspezifische Anweisungen für Claude Code.

## Git Commit Erstellung

### Conventional Commit Messages

Bei der Erstellung von Git Commits soll eine **kurze und prägnante conventional commit message** verwendet werden.

#### Format

```
<type>(<scope>): <subject>

<body>

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>
```

#### Types

- `feat`: Neue Funktionalität
- `fix`: Bugfix
- `refactor`: Code-Refactoring ohne Funktionsänderung
- `docs`: Dokumentationsänderungen
- `test`: Test-bezogene Änderungen
- `build`: Build-System oder externe Dependencies
- `ci`: CI/CD-Konfiguration
- `chore`: Wartungsarbeiten
- `perf`: Performance-Verbesserungen
- `style`: Code-Formatierung (keine funktionale Änderung)

#### Vorgehensweise

1. **Status prüfen** - Welche Dateien wurden geändert?
   ```bash
   git status
   ```

2. **Diff anzeigen** - Was wurde konkret geändert?
   ```bash
   git diff
   git diff --staged
   ```

3. **Historie prüfen** - Welchen Stil verwenden bisherige Commits?
   ```bash
   git log --oneline -5
   ```

4. **Änderungen stagen**
   ```bash
   git add -A
   ```

5. **Commit mit HEREDOC** - Formatierung beibehalten
   ```bash
   git commit -m "$(cat <<'EOF'
   <type>(<scope>): <kurze beschreibende Zusammenfassung>

   - Detaillierte Änderung 1
   - Detaillierte Änderung 2
   - Detaillierte Änderung 3

   🤖 Generated with [Claude Code](https://claude.com/claude-code)

   Co-Authored-By: Claude <noreply@anthropic.com>
   EOF
   )"
   ```

6. **Erfolg verifizieren**
   ```bash
   git status
   ```

#### Beispiel

```bash
git commit -m "$(cat <<'EOF'
refactor(solution): reorganize projects with solution folders and per-format structure

- Split business areas into separate message format projects (Camt052, Camt053, Camt054, Mt940)
- Add solution folders (Core, Infrastructure, Business Areas, Tests)
- Create individual test projects per component for better isolation
- Remove monolithic Camtify.Tests project
- Update documentation and project structure

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>
EOF
)"
```

#### Richtlinien für den Subject

- **Kurz und prägnant**: Maximal 50-72 Zeichen
- **Imperativ**: "add" nicht "added" oder "adds"
- **Kleinschreibung**: Nach dem Typ/Scope
- **Kein Punkt**: Am Ende des Subjects
- **Aussagekräftig**: Erklärt **was** und **warum**, nicht **wie**

#### Richtlinien für den Body

- Aufzählungszeichen für mehrere Änderungen verwenden
- Kontext liefern, warum die Änderung notwendig war
- Bei komplexen Änderungen Breaking Changes erwähnen
- Referenzen zu Issues/Tickets wenn vorhanden

## Code-Dokumentation

### XML-Kommentare

- **ALLE** XML-Kommentare (summary, remarks, param, returns, etc.) MÜSSEN in **Englisch** verfasst werden
- Dies gilt für alle .cs-Dateien im Projekt
- Deutsche Kommentare sind nur in regulären Code-Kommentaren (`//`) erlaubt, wenn unbedingt nötig

#### Beispiel

```csharp
/// <summary>
/// Represents an ISO 20022 message identifier.
/// </summary>
/// <remarks>
/// The identifier follows the pattern: [business area].[message].[variant].[version]
/// </remarks>
public readonly record struct MessageIdentifier
{
    // Korrekt: XML-Kommentare auf Englisch
}
```

## Dateistruktur

### Eine Klasse pro Datei

- **JEDE** Datei darf nur **EINE** Klasse/Record/Struct/Interface/Enum enthalten
- Der Dateiname MUSS dem Typ-Namen entsprechen
- Beispiel: `MessageIdentifier.cs` enthält nur `MessageIdentifier`

## Naming Conventions

### CancellationToken Parameter

- **ALLE** Parameter vom Typ `CancellationToken` MÜSSEN `cancellationToken` heißen
- Nicht `ct`, `token`, oder andere Abkürzungen verwenden

#### Beispiel

```csharp
// ✅ Korrekt
public async Task ProcessAsync(CancellationToken cancellationToken = default)
{
    // ...
}

// ❌ Falsch
public async Task ProcessAsync(CancellationToken ct = default)
{
    // ...
}
```

## Projekt-Architektur

### Trennung von technischem und fachlichem Code

Code muss klar zwischen **technischen** (Infrastruktur) und **fachlichen** (Domain) Belangen getrennt werden:

#### Camtify.Infrastructure
Enthält **rein technische** Utilities und Cross-Cutting Concerns:
- Generische Extension-Methods (z.B. `EnumExtensions.GetDescription<T>()`)
- Reflection-basierte Hilfsmethoden
- Technische Basisklassen und Interfaces
- Serialization/Deserialization Helpers
- Logging/Caching/Performance Utilities

```csharp
// ✅ Korrekt: Technisch/Generisch → Infrastructure
namespace Camtify.Infrastructure.Extensions;

public static class EnumExtensions
{
    public static string GetDescription<TEnum>(this TEnum value) where TEnum : struct, Enum
    { ... }
}
```

#### Camtify.Domain
Enthält **fachliche/business-spezifische** Logik:
- Domain-Models (Money, Iban, Bic, etc.)
- ISO 20022-spezifische Parser und Konverter
- Business-Validierungsregeln
- Fachliche Enums und Konstanten

```csharp
// ✅ Korrekt: Fachlich/ISO 20022-spezifisch → Domain
namespace Camtify.Domain.Common;

public static class Iso20022EnumExtensions
{
    public static TransactionStatus? ParseTransactionStatus(string? code)
    { ... }
}
```

#### Entscheidungskriterien

| Kriterium | → Infrastructure | → Domain |
|-----------|------------------|----------|
| Generisch verwendbar? | ✅ Ja | ❌ Nein |
| ISO 20022-spezifisch? | ❌ Nein | ✅ Ja |
| Verwendet Reflection/System-APIs? | ✅ Ja | ⚠️ Nur wenn fachlich nötig |
| Kennt Domain-Typen? | ❌ Nein | ✅ Ja |
| Wiederverwendbar in anderen Projekten? | ✅ Ja | ❌ Nein |

#### Beispiel: EnumExtensions

```csharp
// ❌ FALSCH: Alles in einer Klasse
public static class EnumExtensions
{
    public static string GetDescription<T>() { }        // Technisch
    public static TransactionStatus? ParseStatus() { }  // Fachlich
}

// ✅ RICHTIG: Getrennt
// In Camtify.Infrastructure:
public static class EnumExtensions
{
    public static string GetDescription<T>() { }
    public static T? ParseFromDescription<T>() { }
}

// In Camtify.Domain:
public static class Iso20022EnumExtensions
{
    public static TransactionStatus? ParseTransactionStatus() { }
    public static string ToIso20022Code<T>() { }
}
```

## Projekt-Konventionen

Siehe [CONTRIBUTING.md](../CONTRIBUTING.md) für detaillierte Projekt-Konventionen.
