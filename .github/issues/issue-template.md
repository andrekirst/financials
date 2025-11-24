# Issue Template für ISO 20022 Parser Library

Dieses Template zeigt die Struktur für alle Issues.

---

## Standard-Issue-Format

```markdown
## Beschreibung

[Klare Beschreibung was gemacht werden soll und warum]

## Aufgaben

- [ ] Aufgabe 1
- [ ] Aufgabe 2
- [ ] Aufgabe 3

## Akzeptanzkriterien

- Kriterium 1 ist erfüllt
- Kriterium 2 ist erfüllt
- Kriterium 3 ist erfüllt

## Schätzung

X-Yh

## Abhängigkeiten

- Abhängig von #<issue-number> (falls zutreffend)
- Blockiert #<issue-number> (falls zutreffend)

## Technische Details

[Optional: Technische Hinweise, Code-Snippets, Architektur-Überlegungen]

## Referenzen

[Optional: Links zu Dokumentation, Specs, etc.]
```

---

## Beispiel: Issue #1

**Titel:** Solution-Struktur für ISO20022 anlegen

**Labels:** `setup`, `infrastructure`, `priority:high`

**Milestone:** Phase 1: Foundation

**Assignee:** (leer lassen oder zuweisen)

**Body:**

```markdown
## Beschreibung

Erstelle eine .NET 8 Solution mit modularer Struktur für das ISO 20022 Parser-Projekt.

## Aufgaben

- [ ] Solution `ISO20022.sln` erstellen
- [ ] Projekt `ISO20022.Core` anlegen (Interfaces, Abstractions)
- [ ] Projekt `ISO20022.Domain` anlegen (Gemeinsame Models)
- [ ] Projekt `ISO20022.Parsing` anlegen (Parser-Infrastruktur)
- [ ] Projekt `ISO20022.Validation` anlegen (Schema & Business Rules)
- [ ] Projekt `ISO20022.Generation` anlegen (XML Writer)
- [ ] Projekt `ISO20022.Tests` anlegen (Unit Tests)
- [ ] `Directory.Build.props` mit gemeinsamen Settings konfigurieren:
  - Nullable: enable
  - ImplicitUsings: enable
  - LangVersion: latest
  - TargetFramework: net8.0
- [ ] Struktur für separate NuGet-Pakete pro Business Area vorbereiten

## Akzeptanzkriterien

- Solution baut erfolgreich
- Alle Projekte referenzieren korrekt
- Gemeinsame Build-Properties funktionieren

## Schätzung

1-2h

## Abhängigkeiten

- Blockiert #2, #3, #4, #5 (alle anderen Setup-Issues)

## Technische Details

**Solution-Struktur:**

```
ISO20022/
├── ISO20022.sln
├── Directory.Build.props
├── Directory.Packages.props
├── src/
│   ├── Core/
│   │   └── ISO20022.Core/
│   ├── Domain/
│   │   └── ISO20022.Domain/
│   ├── Infrastructure/
│   │   ├── ISO20022.Parsing/
│   │   ├── ISO20022.Validation/
│   │   └── ISO20022.Generation/
│   └── BusinessAreas/
│       ├── ISO20022.Pain/
│       ├── ISO20022.Pacs/
│       └── ISO20022.Camt/
└── tests/
    └── ISO20022.Tests/
```

**Directory.Build.props:**

```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
</Project>
```

## Referenzen

- [.NET Solution Structure Best Practices](https://docs.microsoft.com/en-us/dotnet/core/tutorials/...)
- [Central Package Management](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management)
```

---

## Label-Referenz

Nutzen Sie immer passende Labels:

### Priorität (genau 1)
- `priority:high` - Kritisch für MVP
- `priority:medium` - Wichtig
- `priority:low` - Nice-to-have

### Business Area (0-1)
- `pain`, `pacs`, `camt`, `acmt`, `admi`, `remt`, `head`

### Kategorie (1-3)
- `setup`, `core`, `domain`, `parsing`, `generation`, `validation`
- `pipeline`, `streaming`, `transformation`, `testing`
- `performance`, `documentation`, `samples`
- `di`, `configuration`, `observability`, `error-handling`
- `tooling`, `codegen`, `ci-cd`, `code-quality`

### Typ (optional)
- `bug` - Fehler
- `enhancement` - Verbesserung
- `question` - Frage
- `help wanted` - Hilfe gesucht
- `good first issue` - Für Einsteiger geeignet

---

## Milestone-Zuordnung

| Phase | Issues | Milestone |
|-------|--------|-----------|
| 1 | 1-5 | Phase 1: Foundation |
| 2 | 6-13 | Phase 2: Core Domain |
| 3 | 14-20 | Phase 3: Core Parsing |
| 4 | 21-26 | Phase 4: PAIN Parser |
| 5 | 27-32 | Phase 5: PACS Parser |
| 6 | 33-40 | Phase 6: CAMT Parser |
| 7 | 41-47 | Phase 7: Further Business Areas |
| 8 | 48-56 | Phase 8: Streaming & Pipeline |
| 9 | 57-62 | Phase 9: Schema Validation |
| 10 | 63-69 | Phase 10: XML Generation |
| 11 | 70-75 | Phase 11: Version Transformation |
| 12 | 76-85 | Phase 12: Testing |
| 13 | 86-91 | Phase 13: Performance |
| 14 | 92-96 | Phase 14: Observability |
| 15 | 97-100 | Phase 15: DI & Configuration |
| 16 | 101-107 | Phase 16: Documentation |
| 17 | 108-110 | Phase 17: Code Generation (Optional) |

---

## Best Practices

1. **Beschreibung:** Erkläre **was** und **warum**, nicht **wie**
2. **Aufgaben:** Konkrete, testbare Checkboxen
3. **Akzeptanzkriterien:** Messbare Erfolgs-Definitionen
4. **Schätzung:** Realistisch, inkl. Testing
5. **Abhängigkeiten:** Explizit verlinken
6. **Technische Details:** Code-Snippets, Architektur-Diagramme
7. **Referenzen:** Links zu Specs, Docs, RFCs

---

## Issue-Workflow

```
[Open] → [In Progress] → [Review] → [Done]
   ↓                          ↓
[Blocked]                  [Won't Do]
```

**Status-Labels:**
- `status:open` - Neu, bereit zur Bearbeitung
- `status:in-progress` - Wird gerade bearbeitet
- `status:review` - Code Review/Testing
- `status:blocked` - Blockiert durch andere Issues
- `status:done` - Abgeschlossen
- `status:wont-do` - Wird nicht umgesetzt
