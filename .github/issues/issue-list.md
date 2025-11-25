# ISO 20022 Parser Library - Issue-Liste

> **Projekt:** Hochperformante ISO 20022 Parser-Bibliothek in C#  
> **Ziel:** Alle ISO 20022 Business Areas, Streaming-Verarbeitung, TB-Scale-Dateien  
> **Technologie:** .NET 8+, IAsyncEnumerable, System.Threading.Channels

---

## Übersicht: ISO 20022 Business Areas

| Code | Name | Beschreibung | Priorität |
|------|------|--------------|-----------|
| **PAIN** | Payments Initiation | Zahlungsaufträge (pain.001, pain.002, pain.008, pain.013, pain.014) | 🔴 Hoch |
| **PACS** | Payments Clearing & Settlement | Interbank-Zahlungen (pacs.002, pacs.004, pacs.008, pacs.009, pacs.028) | 🔴 Hoch |
| **CAMT** | Cash Management | Kontoauszüge, Reports (camt.052-054, camt.056, camt.029) | 🔴 Hoch |
| **ACMT** | Account Management | Kontoverwaltung | 🟡 Mittel |
| **ADMI** | Administration | Administrative Messages (admi.007) | 🟡 Mittel |
| **REMT** | Remittance Advice | Zahlungsavise | 🟡 Mittel |
| **HEAD** | Business Application Header | BAH für alle Messages | 🔴 Hoch |
| **SESE** | Securities Settlement | Wertpapierabwicklung | 🟢 Niedrig |
| **SEEV** | Securities Events | Corporate Actions | 🟢 Niedrig |
| **SEMT** | Securities Management | Wertpapierverwaltung | 🟢 Niedrig |
| **SETR** | Securities Trade | Wertpapierhandel | 🟢 Niedrig |
| **SECL** | Securities Clearing | Wertpapier-Clearing | 🟢 Niedrig |
| **COLR** | Collateral Management | Sicherheitenverwaltung | 🟢 Niedrig |
| **FXTR** | Foreign Exchange Trade | Devisenhandel | 🟢 Niedrig |
| **TREA** | Treasury | Treasury-Operationen | 🟢 Niedrig |
| **AUTH** | Authorities | Behördenmeldungen | 🟢 Niedrig |
| **REDA** | Reference Data | Stammdaten | 🟢 Niedrig |

---

## Phase 1: Projekt-Setup & Core-Infrastruktur

### Issue 1: Solution-Struktur für ISO20022 anlegen

**Labels:** `setup`, `infrastructure`, `priority:high`  
**Schätzung:** 1-2h

**Beschreibung:**  
Erstelle eine .NET 8 Solution mit modularer Struktur für das ISO 20022 Parser-Projekt.

**Aufgaben:**
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

**Akzeptanzkriterien:**
- Solution baut erfolgreich
- Alle Projekte referenzieren korrekt
- Gemeinsame Build-Properties funktionieren

---

### Issue 2: Business-Area-Projekt-Template erstellen

**Labels:** `setup`, `documentation`, `priority:high`  
**Schätzung:** 1-2h

**Beschreibung:**  
Erstelle ein Template und Konventionen für Business-Area-spezifische Projekte.

**Aufgaben:**
- [ ] Template-Struktur definieren für `ISO20022.{Area}` Projekte
- [ ] Ordnerstruktur festlegen:
  ```
  ISO20022.Pain/
    Models/
    Parsers/
    Writers/
    Validators/
  ```
- [ ] Namespace-Konventionen dokumentieren
- [ ] Dateibenennung-Konventionen definieren
- [ ] CONTRIBUTING.md mit Entwickler-Guidelines erstellen
- [ ] Beispiel-Projekt `ISO20022.Pain` als Referenz anlegen

**Akzeptanzkriterien:**
- Template ist dokumentiert
- Beispiel-Projekt folgt Template
- CONTRIBUTING.md ist vollständig

---

### Issue 3: NuGet Central Package Management

**Labels:** `setup`, `dependencies`, `priority:medium`  
**Schätzung:** 1h

**Beschreibung:**  
Konfiguriere zentrales Package Management für konsistente Dependency-Versionen.

**Aufgaben:**
- [ ] `Directory.Packages.props` erstellen
- [ ] Core-Dependencies hinzufügen:
  - Microsoft.Extensions.DependencyInjection.Abstractions
  - Microsoft.Extensions.Options
  - Microsoft.Extensions.Logging.Abstractions
  - System.Threading.Channels
- [ ] Test-Dependencies hinzufügen:
  - xUnit
  - FluentAssertions
  - Moq
  - BenchmarkDotNet
- [ ] Package-Struktur für NuGet-Veröffentlichung planen
- [ ] `nuget.config` für Package-Sources konfigurieren

**Akzeptanzkriterien:**
- Alle Projekte nutzen zentrale Versionen
- Keine Version-Konflikte
- Dokumentation der Package-Strategie

---

### Issue 4: Build-Pipeline & CI/CD Setup

**Labels:** `setup`, `ci-cd`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Erstelle GitHub Actions Workflow für automatisierte Builds und Tests.

**Aufgaben:**
- [ ] `.github/workflows/build.yml` erstellen
- [ ] Build-Job konfigurieren:
  - dotnet restore
  - dotnet build
  - dotnet test mit Coverage
- [ ] Matrix-Build für verschiedene OS (ubuntu, windows)
- [ ] Code Coverage Report generieren (Coverlet)
- [ ] Coverage-Upload zu Codecov oder ähnlichem
- [ ] NuGet Pack Job für Release-Branches
- [ ] SonarCloud Integration (optional)
- [ ] Branch Protection Rules dokumentieren

**Akzeptanzkriterien:**
- PR-Builds laufen automatisch
- Tests werden ausgeführt
- Coverage wird reported
- NuGet-Pakete werden erstellt

---

### Issue 5: EditorConfig & Code Analyzers

**Labels:** `setup`, `code-quality`, `priority:medium`  
**Schätzung:** 1h

**Beschreibung:**  
Konfiguriere Code-Style und statische Analyse für konsistente Code-Qualität.

**Aufgaben:**
- [ ] `.editorconfig` mit C#-Conventions erstellen
- [ ] Naming-Conventions definieren
- [ ] Formatting-Rules festlegen
- [ ] Analyzers hinzufügen:
  - Microsoft.CodeAnalysis.NetAnalyzers
  - StyleCop.Analyzers
  - SonarAnalyzer.CSharp
- [ ] `.globalconfig` für Severity-Einstellungen
- [ ] Warnings als Errors konfigurieren (in Release)

**Akzeptanzkriterien:**
- Alle Projekte nutzen gleiche Code-Style-Rules
- Analyzer-Warnings werden angezeigt
- CI-Build schlägt bei Violations fehl

---

## Phase 2: Core Domain Models

### Issue 6: ISO20022 Basis-Interfaces definieren

**Labels:** `core`, `interfaces`, `priority:high`  
**Schätzung:** 1-2h

**Beschreibung:**  
Erstelle die fundamentalen Interfaces für alle ISO 20022 Messages.

**Aufgaben:**
- [ ] Interface `IIso20022Message` erstellen:
  ```csharp
  public interface IIso20022Message
  {
      MessageIdentifier MessageId { get; }
      DateTime CreationDateTime { get; }
  }
  ```
- [ ] Interface `IIso20022Document<TMessage>` erstellen
- [ ] Interface `IMessageIdentifier` definieren
- [ ] Gemeinsame Properties dokumentieren:
  - BusinessArea
  - MessageType
  - Version
- [ ] XML-Comments für alle Interfaces

**Akzeptanzkriterien:**
- Interfaces sind im Core-Projekt
- Vollständige XML-Dokumentation
- Kompiliert ohne Warnings

---

### Issue 7: Business Application Header (BAH) Model

**Labels:** `domain`, `models`, `priority:high`  
**Schätzung:** 2h

**Beschreibung:**  
Implementiere das Business Application Header Model (head.001) als gemeinsames Element.

**Aufgaben:**
- [ ] Klasse `BusinessApplicationHeader` erstellen
- [ ] Properties implementieren:
  - CharacterSet
  - From (Party)
  - To (Party)
  - BusinessMessageIdentifier
  - MessageDefinitionIdentifier
  - CreationDate
  - CopyDuplicate
  - Signature (optional)
- [ ] Record-basierte Implementierung prüfen
- [ ] BAH-Versionen unterscheiden (head.001.001.01, head.001.001.02)
- [ ] Unit Tests für BAH

**Akzeptanzkriterien:**
- BAH-Model ist vollständig
- Unterstützt verschiedene Versionen
- Tests vorhanden

---

### Issue 8: Common Party-Models erstellen

**Labels:** `domain`, `models`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Erstelle wiederverwendbare Party-Models, die von allen Business Areas genutzt werden.

**Aufgaben:**
- [ ] `PartyIdentification` Klasse:
  - Name
  - PostalAddress
  - Identification (Org oder Person)
  - CountryOfResidence
  - ContactDetails
- [ ] `PostalAddress` Klasse:
  - AddressType
  - Department
  - StreetName, BuildingNumber
  - PostCode, TownName
  - CountrySubDivision, Country
  - AddressLine (Array)
- [ ] `ContactDetails` Klasse:
  - NamePrefix, Name
  - PhoneNumber, MobileNumber
  - EmailAddress
- [ ] `OrganisationIdentification` Klasse:
  - AnyBIC, LEI
  - Other (Scheme + Id)
- [ ] `PersonIdentification` Klasse:
  - DateAndPlaceOfBirth
  - Other (Scheme + Id)
- [ ] Unit Tests für alle Models

**Akzeptanzkriterien:**
- Models sind im Domain-Projekt
- Nullable Reference Types korrekt
- Tests decken Edge Cases ab

---

### Issue 9: Common Account-Models erstellen

**Labels:** `domain`, `models`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Erstelle Account-bezogene Models mit Validierung.

**Aufgaben:**
- [ ] `CashAccount` Klasse:
  - Identification
  - Type
  - Currency
  - Name
  - Owner
  - Servicer
- [ ] `AccountIdentification` Klasse:
  - IBAN
  - Other (Scheme + Id)
- [ ] `IBAN` Value Object:
  - Validierung (Länge, Prüfsumme)
  - Parsing aus String
  - Country-Code Extraktion
- [ ] `BIC` Value Object:
  - Validierung (8 oder 11 Zeichen)
  - Institution, Country, Location, Branch
- [ ] `ClearingSystemMemberIdentification` Klasse
- [ ] Unit Tests mit validen/invaliden IBANs und BICs

**Akzeptanzkriterien:**
- IBAN-Validierung funktioniert
- BIC-Validierung funktioniert
- Tests mit echten Beispielen

---

### Issue 10: Common Financial-Models erstellen

**Labels:** `domain`, `models`, `priority:high`  
**Schätzung:** 2h

**Beschreibung:**  
Erstelle Models für Beträge, Währungen und finanzielle Werte.

**Aufgaben:**
- [ ] `ActiveCurrencyAndAmount` (Money) Record:
  ```csharp
  public record Money(decimal Amount, CurrencyCode Currency);
  ```
- [ ] `CurrencyCode` Value Object:
  - ISO 4217 Validierung
  - Statische Instanzen (EUR, USD, GBP, etc.)
- [ ] `ExchangeRate` Klasse:
  - SourceCurrency, TargetCurrency
  - Rate, RateType
  - ContractIdentification
- [ ] `ChargesInformation` Klasse:
  - Amount, Agent
  - ChargeType
- [ ] `DateAndDateTime` Klasse:
  - Unterstützt Date-only und DateTime
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Money-Operationen sind sicher (decimal)
- Currency-Validierung funktioniert
- Immutable Records wo sinnvoll

---

### Issue 11: Common Status/Reason-Models

**Labels:** `domain`, `models`, `enums`, `priority:medium`  
**Schätzung:** 1-2h

**Beschreibung:**  
Erstelle Status- und Reason-Models mit ISO-konformen Codes.

**Aufgaben:**
- [ ] `StatusReasonInformation` Klasse:
  - Originator
  - Reason (Code + Proprietary)
  - AdditionalInformation
- [ ] `TransactionStatus` Enum:
  - ACCP (Accepted)
  - RJCT (Rejected)
  - PDNG (Pending)
  - ACSC (Settled)
  - ACSP (Accepted Settlement in Progress)
- [ ] `ReasonCode` Enum (häufige Codes):
  - AC01 (Incorrect Account Number)
  - AM04 (Insufficient Funds)
  - BE04 (Missing Creditor Address)
  - FF01 (Invalid File Format)
  - etc.
- [ ] Description-Attribute für alle Enum-Werte
- [ ] Extension-Methods für Enum-Descriptions

**Akzeptanzkriterien:**
- Alle häufigen ISO-Codes enthalten
- Descriptions sind aussagekräftig
- Erweiterbar für neue Codes

---

### Issue 12: Remittance Information Models

**Labels:** `domain`, `models`, `priority:medium`  
**Schätzung:** 2h

**Beschreibung:**  
Erstelle Models für Verwendungszweck-Informationen.

**Aufgaben:**
- [ ] `RemittanceInformation` Klasse:
  - Unstructured (Array of strings)
  - Structured (Array of StructuredRemittanceInformation)
- [ ] `StructuredRemittanceInformation` Klasse:
  - ReferredDocumentInformation
  - ReferredDocumentAmount
  - CreditorReferenceInformation
  - Invoicer, Invoicee
  - TaxRemittance
  - AdditionalRemittanceInformation
- [ ] `CreditorReferenceInformation` Klasse:
  - Type (Code + Issuer)
  - Reference
- [ ] `ReferredDocumentInformation` Klasse:
  - Type, Number
  - RelatedDate
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Structured und Unstructured unterstützt
- Creditor Reference parsing
- Tests mit realen Beispielen

---

### Issue 13: Message Identifier Value Object

**Labels:** `domain`, `models`, `priority:high`  
**Schätzung:** 1-2h

**Beschreibung:**  
Implementiere ein Value Object zum Parsen und Verwalten von Message-Identifiern.

**Aufgaben:**
- [ ] `MessageIdentifier` Record erstellen:
  ```csharp
  public record MessageIdentifier(
      string BusinessArea,    // "pain", "camt", "pacs"
      string MessageType,     // "001", "053"
      string Variant,         // "001"
      string Version          // "09"
  );
  ```
- [ ] Parsing aus String: `pain.001.001.09`
- [ ] Parsing aus Namespace: `urn:iso:std:iso:20022:tech:xsd:pain.001.001.09`
- [ ] `ToString()` Override
- [ ] `ToNamespace()` Methode
- [ ] Statische Factory-Methoden
- [ ] Validierung der Komponenten
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Parsing funktioniert zuverlässig
- Namespace-Extraktion korrekt
- Immutable und vergleichbar

---

## Phase 3: Core Parsing-Infrastruktur

### Issue 14: Parser-Interfaces definieren

**Labels:** `core`, `interfaces`, `parsing`, `priority:high`  
**Schätzung:** 1-2h

**Beschreibung:**  
Definiere die generischen Parser-Interfaces für alle Message-Types.

**Aufgaben:**
- [ ] `IIso20022Parser<TDocument>` Interface:
  ```csharp
  public interface IIso20022Parser<TDocument> where TDocument : IIso20022Document
  {
      Task<TDocument> ParseAsync(Stream stream, CancellationToken ct = default);
      Task<TDocument> ParseAsync(string filePath, CancellationToken ct = default);
  }
  ```
- [ ] `IAsyncMessageParser<TMessage>` Interface:
  ```csharp
  public interface IAsyncMessageParser<TMessage>
  {
      IAsyncEnumerable<TMessage> ParseStreamAsync(Stream stream, CancellationToken ct = default);
  }
  ```
- [ ] `IStreamingParser<TEntry>` Interface für Entry-Level-Streaming
- [ ] `ParseOptions` Klasse:
  - ValidateSchema
  - StopOnFirstError
  - IgnoreUnknownElements
- [ ] XML-Documentation

**Akzeptanzkriterien:**
- Interfaces sind generisch und wiederverwendbar
- Async-First Design
- CancellationToken überall

---

### Issue 15: Parser-Factory-Interfaces

**Labels:** `core`, `interfaces`, `factory`, `priority:high`  
**Schätzung:** 1-2h

**Beschreibung:**  
Erstelle Factory-Interfaces für dynamische Parser-Erstellung.

**Aufgaben:**
- [ ] `IParserFactory` Interface:
  ```csharp
  public interface IParserFactory
  {
      IIso20022Parser<TDocument> CreateParser<TDocument>(MessageIdentifier messageId);
      bool SupportsMessage(MessageIdentifier messageId);
  }
  ```
- [ ] `IParserRegistry` Interface:
  ```csharp
  public interface IParserRegistry
  {
      void Register<TDocument>(MessageIdentifier messageId, Func<IIso20022Parser<TDocument>> factory);
      IReadOnlyCollection<MessageIdentifier> SupportedMessages { get; }
  }
  ```
- [ ] `ParserNotFoundException` Exception
- [ ] XML-Documentation

**Akzeptanzkriterien:**
- Factory kann Parser dynamisch erstellen
- Registry ermöglicht Erweiterung
- Nicht-unterstützte Messages werfen Exception

---

### Issue 16: Message-Detector implementieren

**Labels:** `core`, `parsing`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Implementiere automatische Erkennung von ISO 20022 Message-Typen.

**Aufgaben:**
- [ ] `Iso20022MessageDetector` Klasse erstellen
- [ ] Methode `DetectMessageType(Stream)`:
  - Lese XML-Root-Element
  - Extrahiere Namespace
  - Parse MessageIdentifier
- [ ] Unterstützung für:
  - Standalone Messages
  - BAH-wrapped Messages (AppHdr + Document)
- [ ] Fehlerbehandlung:
  - Kein XML
  - Kein ISO 20022 Namespace
  - Unbekannte Version
- [ ] Stream-Position zurücksetzen nach Detection
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Erkennt alle gängigen Message-Types
- Funktioniert mit und ohne BAH
- Stream bleibt lesbar nach Detection

---

### Issue 17: Namespace-Registry erstellen

**Labels:** `core`, `parsing`, `priority:high`  
**Schätzung:** 2h

**Beschreibung:**  
Erstelle eine Registry für alle bekannten ISO 20022 Namespaces.

**Aufgaben:**
- [ ] `Iso20022NamespaceRegistry` Klasse
- [ ] Mapping: Namespace → MessageIdentifier
- [ ] Mapping: MessageIdentifier → Namespace
- [ ] Lade Mappings aus eingebetteter Ressource:
  ```json
  {
    "urn:iso:std:iso:20022:tech:xsd:pain.001.001.09": {
      "area": "pain",
      "type": "001",
      "variant": "001",
      "version": "09"
    }
  }
  ```
- [ ] Runtime-Erweiterung ermöglichen
- [ ] Lazy Loading für Performance
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Alle gängigen Namespaces enthalten
- Erweiterbar zur Laufzeit
- Schneller Lookup

---

### Issue 18: XmlReader-Factory implementieren

**Labels:** `core`, `parsing`, `priority:medium`  
**Schätzung:** 1h

**Beschreibung:**  
Erstelle Factory für optimierte XmlReader-Instanzen.

**Aufgaben:**
- [ ] `Iso20022XmlReaderFactory` Klasse
- [ ] Optimierte `XmlReaderSettings`:
  ```csharp
  new XmlReaderSettings
  {
      Async = true,
      IgnoreWhitespace = true,
      IgnoreComments = true,
      DtdProcessing = DtdProcessing.Prohibit,
      XmlResolver = null,  // XXE Prevention
      MaxCharactersInDocument = 0  // Kein Limit
  }
  ```
- [ ] Factory-Methoden für verschiedene Szenarien:
  - Standard Parsing
  - Validating Parser
  - Large File Parsing
- [ ] Dokumentation der Security-Aspekte
- [ ] Unit Tests

**Akzeptanzkriterien:**
- XXE-Attacken verhindert
- Async-Support aktiviert
- Dokumentierte Security

---

### Issue 19: Abstract Parser-Basisklasse

**Labels:** `core`, `parsing`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Implementiere eine abstrakte Basisklasse für alle ISO 20022 Parser.

**Aufgaben:**
- [ ] `Iso20022ParserBase<TDocument>` abstrakte Klasse
- [ ] Template-Method-Pattern:
  ```csharp
  public async Task<TDocument> ParseAsync(Stream stream, CancellationToken ct)
  {
      using var reader = CreateReader(stream);
      var header = await ParseHeaderAsync(reader, ct);
      var document = await ParseDocumentAsync(reader, ct);
      return CreateResult(header, document);
  }
  ```
- [ ] Gemeinsame Funktionalität:
  - Stream-Handling
  - BAH-Parsing (optional)
  - Namespace-Resolution
  - Error-Collection
- [ ] Abstrakte Methoden für spezifisches Parsing
- [ ] Progress-Reporting-Support
- [ ] Unit Tests mit Mock-Implementation

**Akzeptanzkriterien:**
- Gemeinsame Logik zentralisiert
- Erweiterbar für spezifische Parser
- Error-Handling konsistent

---

### Issue 20: Streaming Parser-Basisklasse

**Labels:** `core`, `parsing`, `streaming`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Implementiere Basisklasse für Streaming-Parser mit IAsyncEnumerable.

**Aufgaben:**
- [ ] `StreamingParserBase<TDocument, TEntry>` abstrakte Klasse
- [ ] IAsyncEnumerable-Implementation:
  ```csharp
  public async IAsyncEnumerable<TEntry> ParseEntriesAsync(
      Stream stream,
      [EnumeratorCancellation] CancellationToken ct = default)
  {
      using var reader = CreateReader(stream);
      while (await MoveToNextEntryAsync(reader, ct))
      {
          yield return await ParseEntryAsync(reader, ct);
      }
  }
  ```
- [ ] Konfigurierbare Element-Namen für Entry-Detection
- [ ] Progress-Reporting (IProgress<ParseProgress>)
- [ ] Memory-effizientes Parsing
- [ ] Unit Tests mit großen Testdaten

**Akzeptanzkriterien:**
- Konstanter Memory-Verbrauch
- Cancellation funktioniert
- Progress wird reported

---

## Phase 4: PAIN (Payments Initiation) Parser

### Issue 21: PAIN Domain Models erstellen

**Labels:** `pain`, `domain`, `models`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Erstelle PAIN-spezifische Domain Models im Projekt `ISO20022.Pain`.

**Aufgaben:**
- [ ] Projekt `ISO20022.Pain` anlegen
- [ ] `CustomerCreditTransferInitiation` (pain.001):
  - GroupHeader
  - PaymentInformation[]
- [ ] `PaymentInformation` Klasse:
  - PaymentInformationIdentification
  - PaymentMethod
  - RequestedExecutionDate
  - Debtor, DebtorAccount, DebtorAgent
  - CreditTransferTransactionInformation[]
- [ ] `CreditTransferTransaction` Klasse:
  - PaymentIdentification
  - Amount
  - Creditor, CreditorAccount, CreditorAgent
  - RemittanceInformation
- [ ] `DirectDebitInstruction` (pain.008)
- [ ] `PaymentStatusReport` (pain.002)
- [ ] Unit Tests für Models

**Akzeptanzkriterien:**
- Alle relevanten PAIN-Strukturen abgebildet
- Nullable korrekt gesetzt
- Tests vorhanden

---

### Issue 22: pain.001 Parser (Credit Transfer) implementieren

**Labels:** `pain`, `parsing`, `priority:high`  
**Schätzung:** 3-4h

**Beschreibung:**  
Implementiere Parser für pain.001 (Customer Credit Transfer Initiation).

**Aufgaben:**
- [ ] `Pain001Parser` Klasse erstellen
- [ ] GroupHeader (GrpHdr) parsen:
  - MessageIdentification
  - CreationDateTime
  - NumberOfTransactions
  - ControlSum
  - InitiatingParty
- [ ] PaymentInformation (PmtInf) parsen
- [ ] CreditTransferTransactionInformation (CdtTrfTxInf) parsen
- [ ] Versionen unterstützen: 003, 008, 009, 010, 011
- [ ] Namespace-Handling pro Version
- [ ] Unit Tests mit Test-XMLs

**Akzeptanzkriterien:**
- Alle Versionen werden geparsed
- Vollständige Datenextraktion
- Tests mit echten Bank-Beispielen

---

### Issue 23: pain.001 Streaming für Bulk-Dateien

**Labels:** `pain`, `parsing`, `streaming`, `priority:high`  
**Schätzung:** 2h

**Beschreibung:**  
Erweitere pain.001 Parser um Streaming-Support für große Bulk-Dateien.

**Aufgaben:**
- [ ] `IAsyncEnumerable<CreditTransferTransaction>` Return-Type
- [ ] Streaming auf Transaction-Ebene:
  ```csharp
  public IAsyncEnumerable<CreditTransferTransaction> StreamTransactionsAsync(
      Stream stream, CancellationToken ct)
  ```
- [ ] GroupHeader separat abrufbar
- [ ] PaymentInformation-Kontext erhalten
- [ ] Memory-Profiling mit großen Dateien (100K+ Transaktionen)
- [ ] Unit Tests für Streaming

**Akzeptanzkriterien:**
- Konstanter Memory-Verbrauch
- 100K Transaktionen verarbeitbar
- PaymentInfo-Kontext verfügbar

---

### Issue 24: pain.002 Parser (Status Report) implementieren

**Labels:** `pain`, `parsing`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Implementiere Parser für pain.002 (Payment Status Report).

**Aufgaben:**
- [ ] `Pain002Parser` Klasse erstellen
- [ ] GroupHeader parsen
- [ ] OriginalGroupInformationAndStatus (OrgnlGrpInfAndSts) parsen:
  - OriginalMessageIdentification
  - OriginalMessageNameIdentification
  - GroupStatus
- [ ] TransactionInformationAndStatus (TxInfAndSts) parsen:
  - OriginalPaymentInformationIdentification
  - OriginalEndToEndIdentification
  - TransactionStatus
  - StatusReasonInformation
- [ ] Status-Code-Mapping (ACCP, RJCT, PDNG, etc.)
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Alle Status-Codes werden erkannt
- Original-Referenzen korrekt
- Reason-Codes extrahiert

---

### Issue 25: pain.008 Parser (Direct Debit) implementieren

**Labels:** `pain`, `parsing`, `priority:high`  
**Schätzung:** 3-4h

**Beschreibung:**  
Implementiere Parser für pain.008 (Customer Direct Debit Initiation).

**Aufgaben:**
- [ ] `Pain008Parser` Klasse erstellen
- [ ] GroupHeader parsen
- [ ] PaymentInformation (PmtInf) parsen:
  - PaymentMethod (DD)
  - RequestedCollectionDate
  - Creditor, CreditorAccount, CreditorAgent
- [ ] DirectDebitTransactionInformation (DrctDbtTxInf) parsen:
  - MandateRelatedInformation
  - CreditorSchemeIdentification
  - DebtorAccount, DebtorAgent
- [ ] Mandate-Informationen:
  - MandateIdentification
  - DateOfSignature
  - AmendmentInformationDetails
- [ ] Versionen unterstützen: 002, 007, 008
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Mandate-Parsing vollständig
- SEPA-spezifische Felder unterstützt
- Tests mit echten Beispielen

---

### Issue 26: pain.013/014 Parser (Request for Payment)

**Labels:** `pain`, `parsing`, `priority:medium`  
**Schätzung:** 2-3h

**Beschreibung:**  
Implementiere Parser für Request for Payment Messages.

**Aufgaben:**
- [ ] `Pain013Parser` (CreditorPaymentActivationRequest) erstellen
- [ ] `Pain014Parser` (CreditorPaymentActivationRequestStatusReport) erstellen
- [ ] Strukturen parsen:
  - GroupHeader
  - PaymentInformation
  - RequestedExecutionDate
- [ ] Status-Handling für pain.014
- [ ] Relevant für Instant Payments (TIPS, RT1)
- [ ] Unit Tests

**Akzeptanzkriterien:**
- RFP-Flow unterstützt
- Status-Codes korrekt
- Instant-Payment-tauglich

---

## Phase 5: PACS (Payments Clearing & Settlement) Parser

### Issue 27: PACS Domain Models erstellen

**Labels:** `pacs`, `domain`, `models`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Erstelle PACS-spezifische Domain Models für Interbank-Zahlungen.

**Aufgaben:**
- [ ] Projekt `ISO20022.Pacs` anlegen
- [ ] `FIToFICustomerCreditTransfer` (pacs.008):
  - GroupHeader
  - CreditTransferTransactionInformation[]
- [ ] `PaymentReturn` (pacs.004):
  - GroupHeader
  - OriginalGroupInformation
  - TransactionInformation[]
- [ ] `FIToFIPaymentStatusReport` (pacs.002):
  - GroupHeader
  - OriginalGroupInformationAndStatus
  - TransactionInformationAndStatus[]
- [ ] `FinancialInstitutionCreditTransfer` (pacs.009)
- [ ] `FIToFIPaymentStatusRequest` (pacs.028)
- [ ] Interbank-spezifische Felder:
  - SettlementInformation
  - ChargesInformation
  - InstructingAgent, InstructedAgent
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Interbank-Strukturen vollständig
- Settlement-Details abgebildet
- Charges-Handling korrekt

---

### Issue 28: pacs.008 Parser (FI Credit Transfer) implementieren

**Labels:** `pacs`, `parsing`, `priority:high`  
**Schätzung:** 3-4h

**Beschreibung:**  
Implementiere Parser für pacs.008 (FI to FI Customer Credit Transfer).

**Aufgaben:**
- [ ] `Pacs008Parser` Klasse erstellen
- [ ] GroupHeader (GrpHdr) parsen:
  - MessageIdentification
  - CreationDateTime
  - NumberOfTransactions
  - SettlementInformation
  - InstructingAgent, InstructedAgent
- [ ] CreditTransferTransactionInformation (CdtTrfTxInf) parsen:
  - PaymentIdentification (InstructionId, EndToEndId, UETR)
  - InterbankSettlementAmount
  - ChargesInformation
  - PreviousInstructingAgents
  - IntermediaryAgents
  - Debtor, DebtorAccount, DebtorAgent
  - Creditor, CreditorAccount, CreditorAgent
- [ ] Versionen: 002, 006, 008, 010
- [ ] Unit Tests

**Akzeptanzkriterien:**
- UETR-Extraktion funktioniert
- Agent-Chain vollständig
- Charges korrekt geparsed

---

### Issue 29: pacs.002 Parser (Status Report) implementieren

**Labels:** `pacs`, `parsing`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Implementiere Parser für pacs.002 (FI to FI Payment Status Report).

**Aufgaben:**
- [ ] `Pacs002Parser` Klasse erstellen
- [ ] GroupHeader parsen
- [ ] OriginalGroupInformationAndStatus parsen
- [ ] TransactionInformationAndStatus (TxInfAndSts) parsen:
  - OriginalGroupInformation
  - OriginalInstructionIdentification
  - OriginalEndToEndIdentification
  - OriginalUETR
  - TransactionStatus
  - StatusReasonInformation[]
  - AcceptanceDateTime
- [ ] Detaillierte Rejection-Reasons extrahieren
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Alle Status-Szenarien abgedeckt
- Rejection-Reasons vollständig
- UETR-Referenzierung korrekt

---

### Issue 30: pacs.004 Parser (Payment Return) implementieren

**Labels:** `pacs`, `parsing`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Implementiere Parser für pacs.004 (Payment Return).

**Aufgaben:**
- [ ] `Pacs004Parser` Klasse erstellen
- [ ] GroupHeader mit SettlementInformation parsen
- [ ] OriginalGroupInformation parsen
- [ ] TransactionInformation parsen:
  - ReturnIdentification
  - OriginalGroupInformation
  - OriginalInstructionIdentification
  - OriginalEndToEndIdentification
  - OriginalUETR
  - ReturnedInterbankSettlementAmount
  - ReturnReasonInformation
- [ ] Return-Reasons (Enum + Proprietary)
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Return-Reasons vollständig
- Beträge korrekt (Original vs Returned)
- Original-Referenzen intakt

---

### Issue 31: pacs.009 Parser (FI Credit Transfer) implementieren

**Labels:** `pacs`, `parsing`, `priority:medium`  
**Schätzung:** 2-3h

**Beschreibung:**  
Implementiere Parser für pacs.009 (Financial Institution Credit Transfer).

**Aufgaben:**
- [ ] `Pacs009Parser` Klasse erstellen
- [ ] Cover-Payments unterstützen
- [ ] Bank-to-Bank Transfers parsen
- [ ] UnderlyingCustomerCreditTransfer (optional)
- [ ] Versionen: 002, 006, 008
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Cover-Payment-Struktur erkannt
- B2B-Transfers korrekt
- Underlying-Info extrahiert

---

### Issue 32: pacs.028 Parser (Status Request) implementieren

**Labels:** `pacs`, `parsing`, `priority:medium`  
**Schätzung:** 2h

**Beschreibung:**  
Implementiere Parser für pacs.028 (FI to FI Payment Status Request).

**Aufgaben:**
- [ ] `Pacs028Parser` Klasse erstellen
- [ ] GroupHeader parsen
- [ ] TransactionInformation parsen:
  - StatusRequestIdentification
  - OriginalGroupInformation
  - OriginalInstructionIdentification
  - OriginalEndToEndIdentification
  - OriginalUETR
- [ ] UETR-basierte Abfragen unterstützen
- [ ] Unit Tests

**Akzeptanzkriterien:**
- UETR-Lookup funktioniert
- Request-Referenzen korrekt
- Tests vorhanden

---

## Phase 6: CAMT (Cash Management) Parser

### Issue 33: CAMT Domain Models erstellen

**Labels:** `camt`, `domain`, `models`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Erstelle CAMT-spezifische Domain Models für Kontoauszüge und Reports.

**Aufgaben:**
- [ ] Projekt `ISO20022.Camt` anlegen
- [ ] `BankToCustomerStatement` (camt.053):
  - GroupHeader
  - Statement[]
- [ ] `Statement` Klasse:
  - Identification
  - CreationDateTime
  - Account
  - Balance[]
  - Entry[]
- [ ] `AccountReport` (camt.052)
- [ ] `DebitCreditNotification` (camt.054)
- [ ] `Balance` Klasse:
  - Type (Opening, Closing, Available, etc.)
  - Amount
  - CreditDebitIndicator
  - Date
- [ ] `Entry` Klasse:
  - EntryReference
  - Amount
  - CreditDebitIndicator
  - Status
  - BookingDate, ValueDate
  - EntryDetails[]
- [ ] `EntryDetails` / `TransactionDetails`
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Statement-Struktur vollständig
- Balance-Types abgedeckt
- Entry-Details tief genug

---

### Issue 34: camt.052 Parser (Intraday Report) implementieren

**Labels:** `camt`, `parsing`, `priority:high`  
**Schätzung:** 3-4h

**Beschreibung:**  
Implementiere Parser für camt.052 (Bank to Customer Account Report).

**Aufgaben:**
- [ ] `Camt052Parser` Klasse erstellen
- [ ] GroupHeader parsen
- [ ] Report (Rpt) parsen:
  - Identification
  - CreationDateTime
  - Account
  - Balance[] (Intraday-Salden)
  - Entry[]
- [ ] Streaming für Ntry-Elemente implementieren
- [ ] Versionen: 002, 006, 008, 010
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Intraday-Balances korrekt
- Streaming funktioniert
- Alle Versionen unterstützt

---

### Issue 35: camt.053 Parser (Statement) implementieren

**Labels:** `camt`, `parsing`, `priority:high`  
**Schätzung:** 3-4h

**Beschreibung:**  
Implementiere Parser für camt.053 (Bank to Customer Statement).

**Aufgaben:**
- [ ] `Camt053Parser` Klasse erstellen
- [ ] GroupHeader parsen
- [ ] Statement (Stmt) parsen:
  - Identification
  - StatementPagination
  - CreationDateTime
  - FromToDate
  - Account
  - Balance[] (Opening, Closing, Available)
  - TotalEntries (Summaries)
  - Entry[]
- [ ] Entry-Details vollständig parsen:
  - TransactionDetails
  - RelatedParties
  - RelatedAgents
  - RemittanceInformation
- [ ] Streaming-Support für große Statements
- [ ] Versionen: 002, 004, 006, 008, 010
- [ ] Performance-Tests

**Akzeptanzkriterien:**
- Vollständiges Statement-Parsing
- Entry-Details komplett
- Streaming für große Dateien

---

### Issue 36: camt.054 Parser (Notification) implementieren

**Labels:** `camt`, `parsing`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Implementiere Parser für camt.054 (Debit Credit Notification).

**Aufgaben:**
- [ ] `Camt054Parser` Klasse erstellen
- [ ] GroupHeader parsen
- [ ] Notification (Ntfctn) parsen:
  - Identification
  - CreationDateTime
  - Account
  - Entry[]
- [ ] Real-time Notification Handling
- [ ] Versionen: 002, 006, 008
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Notifications korrekt geparsed
- Real-time-tauglich
- Tests vorhanden

---

### Issue 37: camt.056 Parser (Cancel Request) implementieren

**Labels:** `camt`, `parsing`, `priority:medium`  
**Schätzung:** 2h

**Beschreibung:**  
Implementiere Parser für camt.056 (FI to FI Payment Cancellation Request).

**Aufgaben:**
- [ ] `Camt056Parser` Klasse erstellen
- [ ] Assignment parsen:
  - Assigner, Assignee
  - CreationDateTime
- [ ] ControlData parsen
- [ ] UnderlyingTransaction parsen:
  - OriginalGroupInformation
  - OriginalPaymentInformation
  - TransactionInformation
- [ ] CancellationReason parsen
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Cancellation-Struktur vollständig
- Reason-Codes extrahiert
- Original-Referenzen korrekt

---

### Issue 38: camt.029 Parser (Resolution) implementieren

**Labels:** `camt`, `parsing`, `priority:medium`  
**Schätzung:** 2h

**Beschreibung:**  
Implementiere Parser für camt.029 (Resolution of Investigation).

**Aufgaben:**
- [ ] `Camt029Parser` Klasse erstellen
- [ ] Assignment parsen
- [ ] Status parsen:
  - ConfirmationStatus
  - CancellationDetails
- [ ] Antwort auf camt.056 verarbeiten
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Resolution-Status korrekt
- Verknüpfung zu Original-Request
- Tests vorhanden

---

### Issue 39: camt.026 Parser (Inquiry) implementieren

**Labels:** `camt`, `parsing`, `priority:low`  
**Schätzung:** 1-2h

**Beschreibung:**  
Implementiere Parser für camt.026 (Unable to Apply).

**Aufgaben:**
- [ ] `Camt026Parser` Klasse erstellen
- [ ] Assignment parsen
- [ ] UnderlyingTransaction parsen
- [ ] MissingOrIncorrectInformation parsen
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Inquiry-Struktur geparsed
- Missing-Info identifiziert
- Tests vorhanden

---

### Issue 40: camt.055 Parser (Cancel RFP) implementieren

**Labels:** `camt`, `parsing`, `priority:low`  
**Schätzung:** 1-2h

**Beschreibung:**  
Implementiere Parser für camt.055 (Customer Payment Cancellation Request).

**Aufgaben:**
- [ ] `Camt055Parser` Klasse erstellen
- [ ] GroupHeader parsen
- [ ] UnderlyingTransaction parsen
- [ ] CancellationReason parsen
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Customer-initiated Cancellation
- Reason-Handling
- Tests vorhanden

---

## Phase 7: Weitere Business Areas

### Issue 41: ACMT Projekt-Setup & Models

**Labels:** `acmt`, `domain`, `setup`, `priority:medium`  
**Schätzung:** 2h

**Beschreibung:**  
Erstelle Projekt und Domain Models für Account Management.

**Aufgaben:**
- [ ] Projekt `ISO20022.Acmt` anlegen
- [ ] `AccountOpeningInstruction` (acmt.001)
- [ ] `AccountModificationInstruction` (acmt.002)
- [ ] `AccountClosingRequest` (acmt.003)
- [ ] Gemeinsame Account-Details
- [ ] Unit Tests für Models

**Akzeptanzkriterien:**
- Projekt-Struktur korrekt
- Models vollständig
- Tests vorhanden

---

### Issue 42: acmt.001-003 Parser implementieren

**Labels:** `acmt`, `parsing`, `priority:medium`  
**Schätzung:** 3-4h

**Beschreibung:**  
Implementiere Parser für Account Management Messages.

**Aufgaben:**
- [ ] `Acmt001Parser` (Account Opening)
- [ ] `Acmt002Parser` (Account Modification)
- [ ] `Acmt003Parser` (Account Closing)
- [ ] Gemeinsame Parsing-Logik extrahieren
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Account-Lifecycle abgedeckt
- Wiederverwendbare Logik
- Tests vorhanden

---

### Issue 43: ADMI Projekt-Setup & Models

**Labels:** `admi`, `domain`, `setup`, `priority:medium`  
**Schätzung:** 1-2h

**Beschreibung:**  
Erstelle Projekt und Models für Administration Messages.

**Aufgaben:**
- [ ] Projekt `ISO20022.Admi` anlegen
- [ ] `MessageReject` (admi.002)
- [ ] `SystemEventNotification` (admi.004)
- [ ] `ReceiptAcknowledgement` (admi.007)
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Admin-Messages abgebildet
- Reject-Reasons vollständig
- Tests vorhanden

---

### Issue 44: admi.002/004/007 Parser implementieren

**Labels:** `admi`, `parsing`, `priority:medium`  
**Schätzung:** 2-3h

**Beschreibung:**  
Implementiere Parser für Administration Messages.

**Aufgaben:**
- [ ] `Admi002Parser` (MessageReject)
- [ ] `Admi004Parser` (SystemEventNotification)
- [ ] `Admi007Parser` (ReceiptAcknowledgement)
- [ ] Reject-Reason-Codes (Enum)
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Rejects werden erkannt
- Events geparsed
- Acknowledgements verarbeitet

---

### Issue 45: REMT Projekt-Setup & Models

**Labels:** `remt`, `domain`, `setup`, `priority:low`  
**Schätzung:** 1-2h

**Beschreibung:**  
Erstelle Projekt und Models für Remittance Advice.

**Aufgaben:**
- [ ] Projekt `ISO20022.Remt` anlegen
- [ ] `RemittanceAdvice` (remt.001)
- [ ] `RemittanceLocation` (remt.002)
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Remittance-Strukturen abgebildet
- Tests vorhanden

---

### Issue 46: remt.001/002 Parser implementieren

**Labels:** `remt`, `parsing`, `priority:low`  
**Schätzung:** 2h

**Beschreibung:**  
Implementiere Parser für Remittance Advice Messages.

**Aufgaben:**
- [ ] `Remt001Parser` (RemittanceAdvice)
- [ ] `Remt002Parser` (RemittanceLocation)
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Remittance-Parsing vollständig
- Tests vorhanden

---

### Issue 47: HEAD Parser (BAH standalone) implementieren

**Labels:** `head`, `parsing`, `priority:high`  
**Schätzung:** 2h

**Beschreibung:**  
Implementiere standalone Parser für Business Application Header.

**Aufgaben:**
- [ ] `HeadParser` Klasse erstellen
- [ ] head.001.001.01 parsen
- [ ] head.001.001.02 parsen
- [ ] BAH-Extraktion aus beliebigen Messages
- [ ] Signature-Handling (optional)
- [ ] Unit Tests

**Akzeptanzkriterien:**
- BAH unabhängig parsebar
- Versionen unterschieden
- Integration mit anderen Parsern

---

## Phase 8: Streaming & Channel-Pipeline

### Issue 48: Generic Pipeline-Interfaces definieren

**Labels:** `pipeline`, `interfaces`, `priority:high`  
**Schätzung:** 1-2h

**Beschreibung:**  
Definiere generische Interfaces für die Processing-Pipeline.

**Aufgaben:**
- [ ] `IPipelineStage<TIn, TOut>` Interface:
  ```csharp
  public interface IPipelineStage<TIn, TOut>
  {
      ChannelReader<TOut> Process(ChannelReader<TIn> input, CancellationToken ct);
  }
  ```
- [ ] `IIso20022Pipeline<TDocument, TEntry>` Interface
- [ ] `PipelineConfiguration` Klasse:
  - MaxParallelism
  - ChannelCapacity
  - ErrorHandling
- [ ] `PipelineResult` Klasse:
  - ProcessedCount
  - FailedCount
  - Duration
  - Errors
- [ ] XML-Documentation

**Akzeptanzkriterien:**
- Generische, wiederverwendbare Interfaces
- Konfigurierbar
- Dokumentiert

---

### Issue 49: Generic Read-Stage implementieren

**Labels:** `pipeline`, `streaming`, `priority:high`  
**Schätzung:** 2h

**Beschreibung:**  
Implementiere die erste Pipeline-Stage für XML-Lesen.

**Aufgaben:**
- [ ] `XmlReadStage<TElement>` Klasse
- [ ] Konfigurierbare Element-Detection:
  - ElementName (z.B. "Ntry", "CdtTrfTxInf")
  - Namespace
- [ ] Output-Channel für gelesene Elemente
- [ ] FileStream mit Async und Buffer
- [ ] Progress-Reporting
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Generisch für alle Element-Types
- Async I/O
- Bounded Channel-Output

---

### Issue 50: Generic Parse-Stage implementieren

**Labels:** `pipeline`, `streaming`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Implementiere die Parse-Stage mit parallelen Workern.

**Aufgaben:**
- [ ] `ParseStage<TElement, TEntry>` Klasse
- [ ] Konfigurierbare Parallelität:
  ```csharp
  public ParseStage(
      Func<TElement, TEntry> parser,
      int maxParallelism = 4)
  ```
- [ ] Multiple Worker-Tasks
- [ ] Error-Channel für Parse-Fehler
- [ ] Metrics (Items/Second)
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Paralleles Parsing
- Error-Isolation
- Konfigurierbar

---

### Issue 51: Generic Validate-Stage implementieren

**Labels:** `pipeline`, `validation`, `priority:high`  
**Schätzung:** 2h

**Beschreibung:**  
Implementiere die Validation-Stage.

**Aufgaben:**
- [ ] `ValidateStage<TEntry>` Klasse
- [ ] Konfigurierbare Validators:
  ```csharp
  public ValidateStage(
      IEnumerable<IValidator<TEntry>> validators,
      bool continueOnError = true)
  ```
- [ ] Error-Routing (separate Channel)
- [ ] Validation-Summary
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Mehrere Validators kombinierbar
- Continue-on-Error Option
- Errors getrennt gesammelt

---

### Issue 52: Generic Transform-Stage implementieren

**Labels:** `pipeline`, `transformation`, `priority:medium`  
**Schätzung:** 2h

**Beschreibung:**  
Implementiere die Transform-Stage für Datenkonvertierung.

**Aufgaben:**
- [ ] `TransformStage<TIn, TOut>` Klasse
- [ ] Konfigurierbarer Mapper:
  ```csharp
  public TransformStage(Func<TIn, TOut> mapper)
  ```
- [ ] Async-Mapper-Support
- [ ] Batch-Transformation Option
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Sync und Async Mapping
- Batch-Support
- Tests vorhanden

---

### Issue 53: Generic Write-Stage implementieren

**Labels:** `pipeline`, `output`, `priority:high`  
**Schätzung:** 2h

**Beschreibung:**  
Implementiere die Write-Stage mit Batching.

**Aufgaben:**
- [ ] `WriteStage<TEntry>` Klasse
- [ ] `IEntryWriter<TEntry>` Interface
- [ ] Konfigurierbares Batching:
  ```csharp
  public WriteStage(
      IEntryWriter<TEntry> writer,
      int batchSize = 100)
  ```
- [ ] Flush-on-Complete
- [ ] Retry-Logik für Writer
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Batched Writing
- Writer austauschbar
- Retry bei Fehlern

---

### Issue 54: Pipeline-Builder erstellen

**Labels:** `pipeline`, `builder`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Erstelle Fluent Builder für Pipeline-Konfiguration.

**Aufgaben:**
- [ ] `Iso20022PipelineBuilder` Klasse:
  ```csharp
  var pipeline = new Iso20022PipelineBuilder()
      .ReadFrom(filePath)
      .ParseAs<Camt053Entry>()
      .Validate(validators)
      .Transform(entry => MapToDto(entry))
      .WriteTo(dbWriter)
      .WithParallelism(4)
      .Build();
  ```
- [ ] Stage-Verkettung
- [ ] Configuration-Validation
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Fluent API funktioniert
- Stages korrekt verbunden
- Validierung bei Build

---

### Issue 55: Pipeline-Orchestrator implementieren

**Labels:** `pipeline`, `orchestration`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Implementiere den zentralen Pipeline-Koordinator.

**Aufgaben:**
- [ ] `PipelineOrchestrator` Klasse
- [ ] Stage-Lifecycle Management:
  - Start aller Stages
  - Channel-Completion Propagation
  - Graceful Shutdown
- [ ] Cancellation-Handling
- [ ] Metrics-Collection
- [ ] Error-Aggregation
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Stages starten/stoppen koordiniert
- Cancellation propagiert
- Errors gesammelt

---

### Issue 56: Backpressure & Throttling implementieren

**Labels:** `pipeline`, `performance`, `priority:medium`  
**Schätzung:** 2h

**Beschreibung:**  
Implementiere Backpressure-Mechanismen und Throttling.

**Aufgaben:**
- [ ] Bounded Channels mit konfigurierbarer Kapazität
- [ ] `FullMode` Strategien:
  - Wait (Standard)
  - DropOldest
  - DropNewest
- [ ] Rate Limiter Integration
- [ ] Memory-Monitoring mit Auto-Throttle
- [ ] Metriken für Queue-Depth
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Backpressure funktioniert
- Memory bleibt stabil
- Metrics verfügbar

---

## Phase 9: Schema-Validierung

### Issue 57: XSD-Download-Automatisierung erstellen

**Labels:** `validation`, `tooling`, `priority:medium`  
**Schätzung:** 2-3h

**Beschreibung:**  
Erstelle Tool zum automatischen Download aller XSD-Schemas.

**Aufgaben:**
- [ ] Konsolen-Tool `ISO20022.SchemaDownloader`
- [ ] Versions-Liste von iso20022.org parsen
- [ ] XSDs pro Business Area downloaden
- [ ] Versionierung der Schemas
- [ ] Checksummen generieren
- [ ] Dokumentation

**Akzeptanzkriterien:**
- Automatischer Download funktioniert
- Alle Business Areas abgedeckt
- Versioniert gespeichert

---

### Issue 58: Schema-Repository erstellen

**Labels:** `validation`, `schemas`, `priority:high`  
**Schätzung:** 2h

**Beschreibung:**  
Organisiere XSD-Schemas in strukturiertem Repository.

**Aufgaben:**
- [ ] Ordnerstruktur definieren:
  ```
  /schemas/
    pain/
      pain.001.001.09.xsd
      pain.001.001.10.xsd
    camt/
      camt.053.001.08.xsd
  ```
- [ ] Index-Datei erstellen (JSON):
  ```json
  {
    "schemas": [
      {
        "messageId": "pain.001.001.09",
        "path": "pain/pain.001.001.09.xsd",
        "namespace": "urn:iso:..."
      }
    ]
  }
  ```
- [ ] Als Embedded Resources einbinden
- [ ] Dokumentation

**Akzeptanzkriterien:**
- Struktur konsistent
- Index vollständig
- Embedded Resources funktionieren

---

### Issue 59: Schema-Cache implementieren

**Labels:** `validation`, `performance`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Implementiere Cache für kompilierte XmlSchemaSet-Objekte.

**Aufgaben:**
- [ ] `Iso20022SchemaCache` Klasse
- [ ] ConcurrentDictionary für Thread-Safety
- [ ] Lazy Loading:
  ```csharp
  public XmlSchemaSet GetSchema(MessageIdentifier messageId)
  {
      return _cache.GetOrAdd(messageId.ToString(), _ => LoadAndCompile(messageId));
  }
  ```
- [ ] Schema-Kompilierung (einmalig)
- [ ] Memory-Management (optional: LRU-Eviction)
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Thread-safe
- Schemas nur einmal kompiliert
- Tests vorhanden

---

### Issue 60: Schema-Resolver implementieren

**Labels:** `validation`, `priority:high`  
**Schätzung:** 2h

**Beschreibung:**  
Implementiere Resolver für automatische Schema-Auswahl.

**Aufgaben:**
- [ ] `Iso20022SchemaResolver` Klasse
- [ ] Schema-Lookup per MessageIdentifier
- [ ] Unterstützung für:
  - Embedded Resources
  - Externe Dateien
  - Remote URLs (optional)
- [ ] Fallback-Strategien
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Automatischer Lookup funktioniert
- Mehrere Quellen unterstützt
- Tests vorhanden

---

### Issue 61: Streaming-Validator implementieren

**Labels:** `validation`, `streaming`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Implementiere Schema-Validierung während des Parsings.

**Aufgaben:**
- [ ] `StreamingSchemaValidator` Klasse
- [ ] XmlReaderSettings mit ValidationType.Schema
- [ ] ValidationEventHandler:
  ```csharp
  settings.ValidationEventHandler += (sender, e) =>
  {
      errors.Add(new ValidationError(e.Message, e.Exception.LineNumber));
  };
  ```
- [ ] Continue-on-Error Option
- [ ] Validation-Summary generieren
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Validierung während Parsing
- Errors mit Position
- Konfigurierbar

---

### Issue 62: Business-Rule-Validator Framework erstellen

**Labels:** `validation`, `business-rules`, `priority:medium`  
**Schätzung:** 2-3h

**Beschreibung:**  
Erstelle Framework für Business-Rule-Validierung.

**Aufgaben:**
- [ ] `IBusinessRule<TMessage>` Interface:
  ```csharp
  public interface IBusinessRule<TMessage>
  {
      string RuleId { get; }
      ValidationSeverity Severity { get; }
      ValidationResult Validate(TMessage message);
  }
  ```
- [ ] `BusinessRuleValidator<TMessage>` Klasse
- [ ] Rule-Composition (AND, OR)
- [ ] Severity-Levels (Error, Warning, Info)
- [ ] Beispiel-Rules für PAIN/CAMT
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Rules kombinierbar
- Severity konfigurierbar
- Beispiele vorhanden

---

## Phase 10: XML-Generierung (Writer)

### Issue 63: Writer-Interfaces definieren

**Labels:** `generation`, `interfaces`, `priority:high`  
**Schätzung:** 1-2h

**Beschreibung:**  
Definiere Interfaces für XML-Writer.

**Aufgaben:**
- [ ] `IIso20022Writer<TDocument>` Interface:
  ```csharp
  public interface IIso20022Writer<TDocument>
  {
      Task WriteAsync(TDocument document, Stream output, CancellationToken ct);
      Task WriteAsync(TDocument document, string filePath, CancellationToken ct);
  }
  ```
- [ ] `IStreamingWriter<TEntry>` Interface
- [ ] `WriterOptions` Klasse:
  - Encoding (UTF-8, UTF-16)
  - Indent (true/false)
  - IndentChars
  - OmitXmlDeclaration
  - NamespaceHandling
- [ ] XML-Documentation

**Akzeptanzkriterien:**
- Interfaces generisch
- Options vollständig
- Dokumentiert

---

### Issue 64: Abstract Writer-Basisklasse implementieren

**Labels:** `generation`, `priority:high`  
**Schätzung:** 2h

**Beschreibung:**  
Implementiere abstrakte Basisklasse für alle Writer.

**Aufgaben:**
- [ ] `Iso20022WriterBase<TDocument>` Klasse
- [ ] Template-Method:
  ```csharp
  public async Task WriteAsync(TDocument document, Stream output, CancellationToken ct)
  {
      await using var writer = CreateWriter(output);
      await WriteDocumentStartAsync(writer, ct);
      await WriteHeaderAsync(document, writer, ct);
      await WriteBodyAsync(document, writer, ct);
      await WriteDocumentEndAsync(writer, ct);
  }
  ```
- [ ] XmlWriter-Factory mit Settings
- [ ] Namespace-Management
- [ ] BAH-Writing (optional)
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Gemeinsame Logik zentralisiert
- Namespace korrekt
- Tests vorhanden

---

### Issue 65: pain.001 Writer implementieren

**Labels:** `pain`, `generation`, `priority:high`  
**Schätzung:** 3-4h

**Beschreibung:**  
Implementiere Writer für pain.001 Credit Transfer Initiation.

**Aufgaben:**
- [ ] `Pain001Writer` Klasse
- [ ] GroupHeader schreiben
- [ ] PaymentInformation schreiben
- [ ] CreditTransferTransactionInformation schreiben
- [ ] Streaming für Bulk-Dateien
- [ ] Versionen: 003, 009, 010
- [ ] Unit Tests + Roundtrip-Tests

**Akzeptanzkriterien:**
- Valides XML generiert
- Schema-konform
- Roundtrip erfolgreich

---

### Issue 66: pain.008 Writer implementieren

**Labels:** `pain`, `generation`, `priority:medium`  
**Schätzung:** 2-3h

**Beschreibung:**  
Implementiere Writer für pain.008 Direct Debit Initiation.

**Aufgaben:**
- [ ] `Pain008Writer` Klasse
- [ ] Mandate-Informationen schreiben
- [ ] SEPA-spezifische Felder
- [ ] Versionen: 002, 008
- [ ] Unit Tests + Roundtrip-Tests

**Akzeptanzkriterien:**
- Mandate-Handling korrekt
- SEPA-konform
- Tests vorhanden

---

### Issue 67: camt.053 Writer implementieren

**Labels:** `camt`, `generation`, `priority:medium`  
**Schätzung:** 3-4h

**Beschreibung:**  
Implementiere Writer für camt.053 Bank Statement.

**Aufgaben:**
- [ ] `Camt053Writer` Klasse
- [ ] Statement mit Balances schreiben
- [ ] Entries schreiben
- [ ] EntryDetails schreiben
- [ ] Streaming für große Statements
- [ ] Versionen: 002, 008
- [ ] Unit Tests + Roundtrip-Tests

**Akzeptanzkriterien:**
- Vollständige Statements
- Streaming funktioniert
- Roundtrip erfolgreich

---

### Issue 68: pacs.008 Writer implementieren

**Labels:** `pacs`, `generation`, `priority:medium`  
**Schätzung:** 2-3h

**Beschreibung:**  
Implementiere Writer für pacs.008 FI to FI Credit Transfer.

**Aufgaben:**
- [ ] `Pacs008Writer` Klasse
- [ ] SettlementInformation schreiben
- [ ] Agent-Chain schreiben
- [ ] ChargesInformation schreiben
- [ ] Versionen: 002, 008
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Interbank-Struktur korrekt
- UETR enthalten
- Tests vorhanden

---

### Issue 69: Generic Fluent Builder implementieren

**Labels:** `generation`, `builder`, `priority:medium`  
**Schätzung:** 2-3h

**Beschreibung:**  
Erstelle generisches Builder-Pattern für Message-Erstellung.

**Aufgaben:**
- [ ] `Iso20022Builder<TDocument>` Basisklasse
- [ ] Fluent API Design:
  ```csharp
  var message = new Pain001Builder()
      .WithMessageId("MSG001")
      .WithCreationDateTime(DateTime.UtcNow)
      .AddPaymentInfo(pi => pi
          .WithDebtorAccount("DE89370400440532013000")
          .AddTransaction(tx => tx
              .WithAmount(100.00m, "EUR")
              .WithCreditorAccount("FR7630006000011234567890189")
          )
      )
      .Build();
  ```
- [ ] Validation in Build()
- [ ] Beispiele für PAIN, CAMT
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Intuitive API
- Validation bei Build
- Beispiele dokumentiert

---

## Phase 11: Versions-Transformation

### Issue 70: Transformer-Interfaces definieren

**Labels:** `transformation`, `interfaces`, `priority:medium`  
**Schätzung:** 1-2h

**Beschreibung:**  
Definiere Interfaces für Versions-Transformation.

**Aufgaben:**
- [ ] `IMessageTransformer<TSource, TTarget>` Interface
- [ ] `IVersionUpgrader<TMessage>` Interface
- [ ] `IVersionDowngrader<TMessage>` Interface
- [ ] `TransformationResult` Klasse:
  - Target
  - Warnings (nicht-mappbare Felder)
  - DataLoss (entfernte Felder)
- [ ] XML-Documentation

**Akzeptanzkriterien:**
- Interfaces flexibel
- DataLoss trackbar
- Dokumentiert

---

### Issue 71: Version-Mapping-Registry erstellen

**Labels:** `transformation`, `mapping`, `priority:medium`  
**Schätzung:** 2h

**Beschreibung:**  
Erstelle Registry für Feld-Mappings zwischen Versionen.

**Aufgaben:**
- [ ] `VersionMappingRegistry` Klasse
- [ ] Mapping-Metadaten definieren:
  - Renamed (altes Feld → neues Feld)
  - Added (neue Felder)
  - Removed (entfernte Felder)
  - Restructured (geänderte Struktur)
- [ ] JSON-basierte Konfiguration
- [ ] API für Mapping-Lookup
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Mappings vollständig
- Lookup funktioniert
- Tests vorhanden

---

### Issue 72: AutoMapper-Profile Generator erstellen

**Labels:** `transformation`, `tooling`, `priority:low`  
**Schätzung:** 2-3h

**Beschreibung:**  
Erstelle Tool zur automatischen Profile-Generierung.

**Aufgaben:**
- [ ] Generator-Tool erstellen
- [ ] Input: Version-Mapping-Registry
- [ ] Output: AutoMapper Profile C#-Code
- [ ] Custom-Resolver für komplexe Mappings
- [ ] Dokumentation

**Akzeptanzkriterien:**
- Automatische Generierung
- Manuelles Override möglich
- Dokumentiert

---

### Issue 73: PAIN Version-Transformers implementieren

**Labels:** `pain`, `transformation`, `priority:medium`  
**Schätzung:** 3-4h

**Beschreibung:**  
Implementiere Transformers für PAIN-Versionen.

**Aufgaben:**
- [ ] pain.001 V03 → V09 Transformer
- [ ] pain.001 V09 → V10 Transformer
- [ ] pain.008 V02 → V08 Transformer
- [ ] Bidirektionale Transformation
- [ ] DataLoss-Warnings
- [ ] Unit Tests + Roundtrip-Tests

**Akzeptanzkriterien:**
- Daten bleiben erhalten
- Warnings bei DataLoss
- Roundtrip erfolgreich

---

### Issue 74: CAMT Version-Transformers implementieren

**Labels:** `camt`, `transformation`, `priority:medium`  
**Schätzung:** 3-4h

**Beschreibung:**  
Implementiere Transformers für CAMT-Versionen.

**Aufgaben:**
- [ ] camt.053 V02 → V08 Transformer
- [ ] camt.053 V08 → V10 Transformer
- [ ] Strukturelle Änderungen handlen
- [ ] Entry-Details Mapping
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Komplexe Strukturen transformiert
- Tests mit echten Daten
- DataLoss dokumentiert

---

### Issue 75: MT↔MX Konverter-Framework erstellen

**Labels:** `transformation`, `mt`, `priority:low`  
**Schätzung:** 4h

**Beschreibung:**  
Erstelle Framework für MT (SWIFT FIN) zu MX (ISO 20022) Konvertierung.

**Aufgaben:**
- [ ] `IMTMXConverter` Interface
- [ ] MT-Parser-Abstraktion (extern oder eigen)
- [ ] MT940 → camt.053 Konverter als Beispiel
- [ ] Mapping-Dokumentation
- [ ] Unit Tests

**Akzeptanzkriterien:**
- Framework erweiterbar
- MT940-Beispiel funktioniert
- Mapping dokumentiert

---

## Phase 12: Testing

### Issue 76: Test-Infrastruktur Setup

**Labels:** `testing`, `setup`, `priority:high`  
**Schätzung:** 1-2h

**Beschreibung:**  
Konfiguriere umfassende Test-Infrastruktur.

**Aufgaben:**
- [ ] xUnit als Test-Framework
- [ ] FluentAssertions für Assertions
- [ ] Moq für Mocking
- [ ] AutoFixture für Test-Daten
- [ ] Verify für Snapshot-Testing
- [ ] Test-Projekt-Struktur pro Business Area
- [ ] Test-Kategorien definieren (Unit, Integration, Performance)

**Akzeptanzkriterien:**
- Alle Packages installiert
- Struktur konsistent
- Kategorien funktionieren

---

### Issue 77: Test-XML-Repository aufbauen

**Labels:** `testing`, `test-data`, `priority:high`  
**Schätzung:** 3-4h

**Beschreibung:**  
Sammle und erstelle Test-XMLs für alle unterstützten Messages.

**Aufgaben:**
- [ ] Struktur definieren:
  ```
  /TestData/
    pain/
      pain.001/
        v09/
          valid/
          invalid/
    camt/
      camt.053/
  ```
- [ ] Valide Beispiele pro Message/Version
- [ ] Invalide Beispiele (Schema-Fehler)
- [ ] Edge Cases (leere Listen, optionale Felder)
- [ ] Als Embedded Resources einbinden
- [ ] README pro Ordner

**Akzeptanzkriterien:**
- Alle Messages abgedeckt
- Valid und Invalid vorhanden
- Dokumentiert

---

### Issue 78: Test-Data-Generator erstellen

**Labels:** `testing`, `tooling`, `priority:medium`  
**Schätzung:** 3-4h

**Beschreibung:**  
Implementiere Generator für synthetische Test-Daten.

**Aufgaben:**
- [ ] `Iso20022TestDataGenerator` Klasse
- [ ] Konfigurierbare Komplexität:
  - Anzahl Transaktionen
  - Optional Felder füllen
  - Random vs deterministische Daten
- [ ] Generierung für PAIN, CAMT, PACS
- [ ] Large-File-Generierung (100K+ Transaktionen)
- [ ] CLI-Tool

**Akzeptanzkriterien:**
- Generiert valide XMLs
- Skaliert für große Dateien
- CLI nutzbar

---

### Issue 79: Parser Unit Tests (PAIN) schreiben

**Labels:** `pain`, `testing`, `priority:high`  
**Schätzung:** 3-4h

**Beschreibung:**  
Schreibe umfassende Unit Tests für PAIN-Parser.

**Aufgaben:**
- [ ] pain.001 Parser Tests:
  - Valide Dokumente (alle Versionen)
  - Einzelne Felder
  - Edge Cases
- [ ] pain.002 Parser Tests
- [ ] pain.008 Parser Tests
- [ ] pain.013/014 Parser Tests
- [ ] Error-Handling Tests
- [ ] Code Coverage > 80%

**Akzeptanzkriterien:**
- Alle Parser getestet
- Edge Cases abgedeckt
- Coverage erreicht

---

### Issue 80: Parser Unit Tests (PACS) schreiben

**Labels:** `pacs`, `testing`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Schreibe Unit Tests für PACS-Parser.

**Aufgaben:**
- [ ] pacs.008 Parser Tests
- [ ] pacs.002 Parser Tests
- [ ] pacs.004 Parser Tests
- [ ] pacs.009 Parser Tests
- [ ] pacs.028 Parser Tests
- [ ] Interbank-spezifische Felder testen
- [ ] Code Coverage > 80%

**Akzeptanzkriterien:**
- Alle Parser getestet
- Agent-Chains korrekt
- Coverage erreicht

---

### Issue 81: Parser Unit Tests (CAMT) schreiben

**Labels:** `camt`, `testing`, `priority:high`  
**Schätzung:** 3-4h

**Beschreibung:**  
Schreibe Unit Tests für CAMT-Parser.

**Aufgaben:**
- [ ] camt.052 Parser Tests
- [ ] camt.053 Parser Tests:
  - Vollständige Statements
  - Balances
  - Entry-Details
- [ ] camt.054 Parser Tests
- [ ] camt.056/029 Parser Tests
- [ ] Streaming-Verhalten testen
- [ ] Code Coverage > 80%

**Akzeptanzkriterien:**
- Alle Parser getestet
- Streaming funktioniert
- Coverage erreicht

---

### Issue 82: Streaming & Pipeline Tests schreiben

**Labels:** `pipeline`, `testing`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Teste IAsyncEnumerable und Channel-Pipeline.

**Aufgaben:**
- [ ] IAsyncEnumerable-Verhalten:
  - Partial Iteration
  - Cancellation
  - Exception Propagation
- [ ] Memory-Konstanz verifizieren
- [ ] Channel-Backpressure testen
- [ ] Stage-Isolation testen
- [ ] Pipeline-Orchestration testen

**Akzeptanzkriterien:**
- Async-Verhalten korrekt
- Memory stabil
- Stages isoliert

---

### Issue 83: Writer Roundtrip Tests schreiben

**Labels:** `generation`, `testing`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Teste Parse→Write→Parse Roundtrips.

**Aufgaben:**
- [ ] Roundtrip für pain.001
- [ ] Roundtrip für pain.008
- [ ] Roundtrip für camt.053
- [ ] Roundtrip für pacs.008
- [ ] Datenerhalt verifizieren
- [ ] Schema-Validierung des Outputs

**Akzeptanzkriterien:**
- Daten bleiben erhalten
- Output ist schema-konform
- Alle Versionen

---

### Issue 84: Schema-Validation Tests schreiben

**Labels:** `validation`, `testing`, `priority:medium`  
**Schätzung:** 2h

**Beschreibung:**  
Teste Schema-Validierung gegen offizielle XSDs.

**Aufgaben:**
- [ ] Valide Dokumente passieren
- [ ] Invalide Dokumente werden erkannt
- [ ] Error-Positionen korrekt
- [ ] Performance-Test (große Dateien)
- [ ] Cache-Verhalten testen

**Akzeptanzkriterien:**
- Validierung korrekt
- Errors aussagekräftig
- Performance akzeptabel

---

### Issue 85: Integration Tests erstellen

**Labels:** `testing`, `integration`, `priority:medium`  
**Schätzung:** 2-3h

**Beschreibung:**  
Erstelle End-to-End Integration Tests.

**Aufgaben:**
- [ ] File → Parse → Validate → Transform → Write → Verify
- [ ] Pipeline mit allen Stages
- [ ] Real-World-Szenarien:
  - Bulk-Payment-File verarbeiten
  - Statement reconciliieren
- [ ] Docker-basierte Tests (optional)

**Akzeptanzkriterien:**
- E2E funktioniert
- Real-World-Szenarien abgedeckt
- Dokumentiert

---

## Phase 13: Performance & Benchmarking

### Issue 86: BenchmarkDotNet Setup

**Labels:** `performance`, `setup`, `priority:medium`  
**Schätzung:** 1h

**Beschreibung:**  
Erstelle Benchmark-Projekt für Performance-Tests.

**Aufgaben:**
- [ ] Projekt `ISO20022.Benchmarks` erstellen
- [ ] BenchmarkDotNet konfigurieren
- [ ] MemoryDiagnoser aktivieren
- [ ] Multiple Runtimes (.NET 8, 9)
- [ ] Baseline-Benchmarks

**Akzeptanzkriterien:**
- Projekt konfiguriert
- Benchmarks laufen
- Memory wird gemessen

---

### Issue 87: Large-File-Generator erstellen

**Labels:** `performance`, `tooling`, `priority:medium`  
**Schätzung:** 2-3h

**Beschreibung:**  
Erstelle Tool zur Generierung großer Test-Dateien.

**Aufgaben:**
- [ ] Generator für pain.001 mit N Transaktionen
- [ ] Generator für camt.053 mit N Entries
- [ ] Konfigurierbare Größen: 100K, 1M, 10M
- [ ] Deterministische Generierung (Seed)
- [ ] CLI-Interface

**Akzeptanzkriterien:**
- Generiert schnell
- Dateien sind valide
- Reproduzierbar

---

### Issue 88: Parser-Comparison Benchmarks erstellen

**Labels:** `performance`, `parsing`, `priority:medium`  
**Schätzung:** 2-3h

**Beschreibung:**  
Benchmark verschiedene Parsing-Ansätze.

**Aufgaben:**
- [ ] XmlReader vs XDocument vs XmlSerializer
- [ ] Verschiedene Dateigrößen (1KB, 1MB, 100MB)
- [ ] Verschiedene Message-Types
- [ ] Memory-Verbrauch messen
- [ ] Ergebnisse dokumentieren

**Akzeptanzkriterien:**
- Vergleichbare Ergebnisse
- Memory dokumentiert
- Empfehlungen abgeleitet

---

### Issue 89: Memory-Profiling durchführen

**Labels:** `performance`, `memory`, `priority:medium`  
**Schätzung:** 2h

**Beschreibung:**  
Führe detailliertes Memory-Profiling durch.

**Aufgaben:**
- [ ] Memory-Verbrauch bei verschiedenen Dateigrößen
- [ ] GC-Collections zählen
- [ ] Allocations analysieren
- [ ] Streaming vs In-Memory vergleichen
- [ ] Optimierungspotenziale identifizieren

**Akzeptanzkriterien:**
- Konstanter Speicher bei Streaming
- GC-Druck minimiert
- Optimierungen dokumentiert

---

### Issue 90: Pipeline-Throughput Benchmarks erstellen

**Labels:** `performance`, `pipeline`, `priority:medium`  
**Schätzung:** 2-3h

**Beschreibung:**  
Benchmark die Channel-Pipeline.

**Aufgaben:**
- [ ] Messages/Sekunde messen
- [ ] Latenz pro Stage messen
- [ ] Parallelitäts-Skalierung testen
- [ ] Verschiedene Channel-Kapazitäten
- [ ] Bottleneck-Analyse

**Akzeptanzkriterien:**
- Throughput dokumentiert
- Skalierung verstanden
- Bottlenecks identifiziert

---

### Issue 91: Version-Comparison Benchmarks erstellen

**Labels:** `performance`, `priority:low`  
**Schätzung:** 2h

**Beschreibung:**  
Vergleiche Performance zwischen Versionen.

**Aufgaben:**
- [ ] V02 vs V08 vs V10 für camt.053
- [ ] V03 vs V09 vs V10 für pain.001
- [ ] Parsing-Zeit vergleichen
- [ ] Memory vergleichen
- [ ] Erklärungen für Unterschiede

**Akzeptanzkriterien:**
- Vergleich dokumentiert
- Unterschiede erklärt
- Empfehlungen

---

## Phase 14: Error Handling & Observability

### Issue 92: Exception-Hierarchie erstellen

**Labels:** `error-handling`, `priority:high`  
**Schätzung:** 1-2h

**Beschreibung:**  
Erstelle konsistente Exception-Hierarchie.

**Aufgaben:**
- [ ] `Iso20022Exception` Basisklasse
- [ ] `Iso20022ParsingException`:
  - LineNumber, LinePosition
  - ElementPath
  - InnerException
- [ ] `Iso20022ValidationException`:
  - ValidationErrors
- [ ] `Iso20022TransformationException`
- [ ] `Iso20022WriterException`
- [ ] XML-Documentation

**Akzeptanzkriterien:**
- Hierarchie konsistent
- Context-Information vorhanden
- Dokumentiert

---

### Issue 93: Structured Logging integrieren

**Labels:** `logging`, `observability`, `priority:high`  
**Schätzung:** 1-2h

**Beschreibung:**  
Integriere strukturiertes Logging.

**Aufgaben:**
- [ ] Microsoft.Extensions.Logging nutzen
- [ ] Log-Events definieren:
  - ParseStarted, ParseCompleted
  - ValidationError
  - TransformationWarning
- [ ] Strukturierte Properties:
  - {MessageId}, {BusinessArea}
  - {Duration}, {EntryCount}
- [ ] EventIds definieren
- [ ] Beispiel-Konfiguration (Serilog)

**Akzeptanzkriterien:**
- Strukturierte Logs
- Konsistente Events
- Beispiel-Config

---

### Issue 94: Metrics & Telemetry implementieren

**Labels:** `metrics`, `observability`, `priority:medium`  
**Schätzung:** 2h

**Beschreibung:**  
Integriere Metrics für Monitoring.

**Aufgaben:**
- [ ] System.Diagnostics.Metrics nutzen
- [ ] Meter `ISO20022.Parser` erstellen
- [ ] Counter:
  - messages_parsed_total
  - validation_errors_total
- [ ] Histogram:
  - parse_duration_seconds
  - message_size_bytes
- [ ] Gauge:
  - pipeline_queue_depth
- [ ] Dokumentation

**Akzeptanzkriterien:**
- Metrics definiert
- Exportierbar (Prometheus)
- Dokumentiert

---

### Issue 95: OpenTelemetry Integration

**Labels:** `observability`, `tracing`, `priority:medium`  
**Schätzung:** 2-3h

**Beschreibung:**  
Füge OpenTelemetry Tracing hinzu.

**Aufgaben:**
- [ ] ActivitySource erstellen
- [ ] Activities für:
  - Parse
  - Validate
  - Transform
  - Write
- [ ] TraceContext propagieren
- [ ] Spans mit Attributen anreichern
- [ ] Beispiel-Konfiguration

**Akzeptanzkriterien:**
- Distributed Tracing funktioniert
- Context propagiert
- Beispiel vorhanden

---

### Issue 96: Health Checks implementieren

**Labels:** `observability`, `health`, `priority:medium`  
**Schätzung:** 1-2h

**Beschreibung:**  
Implementiere Health Checks für Monitoring.

**Aufgaben:**
- [ ] `IHealthCheck` Implementierungen:
  - SchemaAvailabilityHealthCheck
  - ParserRegistryHealthCheck
  - PipelineHealthCheck
- [ ] Health-Status:
  - Healthy, Degraded, Unhealthy
- [ ] Detaillierte Beschreibungen
- [ ] Beispiel-Integration

**Akzeptanzkriterien:**
- Health Checks funktionieren
- Status aussagekräftig
- Integrierbar

---

## Phase 15: DI & Konfiguration

### Issue 97: Options-Klassen erstellen

**Labels:** `configuration`, `priority:high`  
**Schätzung:** 1-2h

**Beschreibung:**  
Erstelle Options-Klassen für Konfiguration.

**Aufgaben:**
- [ ] `Iso20022Options` Klasse:
  - SupportedBusinessAreas
  - SchemaLocation
  - DefaultVersion
- [ ] `ParserOptions` Klasse:
  - ValidateSchema
  - StopOnFirstError
  - MaxFileSizeBytes
- [ ] `ValidationOptions` Klasse
- [ ] `PipelineOptions` Klasse
- [ ] DataAnnotations für Validierung

**Akzeptanzkriterien:**
- Options vollständig
- Validierung funktioniert
- Dokumentiert

---

### Issue 98: ServiceCollection Extensions erstellen

**Labels:** `di`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Erstelle Extension-Methods für DI-Registrierung.

**Aufgaben:**
- [ ] `AddIso20022()` Basis-Extension:
  ```csharp
  services.AddIso20022(options => {
      options.SchemaLocation = "./Schemas";
  });
  ```
- [ ] `AddIso20022Pain()` für PAIN-Parser
- [ ] `AddIso20022Camt()` für CAMT-Parser
- [ ] `AddIso20022Pacs()` für PACS-Parser
- [ ] Modulare Registrierung
- [ ] Options-Binding aus Configuration

**Akzeptanzkriterien:**
- Modulare Registrierung
- Options funktionieren
- Dokumentiert

---

### Issue 99: Keyed Services Setup

**Labels:** `di`, `.net8`, `priority:medium`  
**Schätzung:** 1-2h

**Beschreibung:**  
Nutze .NET 8 Keyed Services für Parser-Registrierung.

**Aufgaben:**
- [ ] Keyed Registration:
  ```csharp
  services.AddKeyedTransient<IIso20022Parser, Pain001Parser>("pain.001.001.09");
  ```
- [ ] Factory mit Keyed Services
- [ ] Injection mit `[FromKeyedServices]`
- [ ] Dokumentation

**Akzeptanzkriterien:**
- Keyed Services funktionieren
- Factory nutzt Keys
- Beispiele vorhanden

---

### Issue 100: Configuration Binding implementieren

**Labels:** `configuration`, `priority:medium`  
**Schätzung:** 1-2h

**Beschreibung:**  
Implementiere Binding aus verschiedenen Quellen.

**Aufgaben:**
- [ ] appsettings.json Binding:
  ```json
  {
    "Iso20022": {
      "SchemaLocation": "./Schemas",
      "Parser": {
        "ValidateSchema": true
      }
    }
  }
  ```
- [ ] Environment Variables Support
- [ ] Validierung beim Start
- [ ] Dokumentation

**Akzeptanzkriterien:**
- JSON Binding funktioniert
- Env Vars funktionieren
- Validierung aktiv

---

## Phase 16: Dokumentation

### Issue 101: README & Getting Started erstellen

**Labels:** `documentation`, `priority:high`  
**Schätzung:** 2-3h

**Beschreibung:**  
Schreibe umfassende README-Dokumentation.

**Aufgaben:**
- [ ] README.md:
  - Features-Übersicht
  - Installation (NuGet)
  - Quick Start pro Business Area
  - Beispiel-Code
- [ ] Badges: Build, NuGet, Coverage
- [ ] Lizenz-Information
- [ ] Contributing-Link
- [ ] Changelog-Link

**Akzeptanzkriterien:**
- README vollständig
- Beispiele funktionieren
- Professionell

---

### Issue 102: API-Dokumentation erstellen

**Labels:** `documentation`, `api-docs`, `priority:medium`  
**Schätzung:** 3-4h

**Beschreibung:**  
Erstelle vollständige API-Dokumentation.

**Aufgaben:**
- [ ] XML-Comments für alle öffentlichen APIs
- [ ] DocFX oder ähnliches Tool einrichten
- [ ] Namespace-Dokumentation
- [ ] Konzeptuelle Dokumentation:
  - Architektur-Übersicht
  - Versionierungs-Strategie
  - Performance-Tipps
- [ ] Automatische Generierung in CI

**Akzeptanzkriterien:**
- Alle APIs dokumentiert
- Docs werden generiert
- Online verfügbar

---

### Issue 103: Sample: PAIN Processing erstellen

**Labels:** `documentation`, `samples`, `priority:medium`  
**Schätzung:** 2h

**Beschreibung:**  
Erstelle Beispiel-Projekt für PAIN-Verarbeitung.

**Aufgaben:**
- [ ] Sample-Projekt `ISO20022.Samples.Pain`
- [ ] Use Cases:
  - pain.001 parsen
  - Validieren
  - pain.002 Status-Report verarbeiten
- [ ] Dokumentierter Code
- [ ] README im Sample-Ordner

**Akzeptanzkriterien:**
- Sample läuft
- Code gut kommentiert
- README vorhanden

---

### Issue 104: Sample: CAMT Processing erstellen

**Labels:** `documentation`, `samples`, `priority:medium`  
**Schätzung:** 2h

**Beschreibung:**  
Erstelle Beispiel-Projekt für CAMT-Verarbeitung.

**Aufgaben:**
- [ ] Sample-Projekt `ISO20022.Samples.Camt`
- [ ] Use Cases:
  - camt.053 streamen
  - Entries in Console/Datenbank
  - Balances extrahieren
- [ ] Dokumentierter Code
- [ ] README

**Akzeptanzkriterien:**
- Streaming funktioniert
- Code dokumentiert
- README vorhanden

---

### Issue 105: Sample: Pipeline Processing erstellen

**Labels:** `documentation`, `samples`, `priority:medium`  
**Schätzung:** 2-3h

**Beschreibung:**  
Erstelle Beispiel-Projekt für Pipeline-Verarbeitung.

**Aufgaben:**
- [ ] Sample-Projekt `ISO20022.Samples.Pipeline`
- [ ] Komplette Pipeline:
  - Read → Parse → Validate → Transform → Write
- [ ] Metriken anzeigen
- [ ] Error-Handling demonstrieren
- [ ] Dokumentierter Code

**Akzeptanzkriterien:**
- Pipeline funktioniert
- Metriken sichtbar
- Errors gehandled

---

### Issue 106: Sample: Version Migration erstellen

**Labels:** `documentation`, `samples`, `priority:low`  
**Schätzung:** 2h

**Beschreibung:**  
Erstelle Beispiel für Versions-Migration.

**Aufgaben:**
- [ ] Sample-Projekt `ISO20022.Samples.Migration`
- [ ] Use Cases:
  - pain.001 V03 → V09
  - camt.053 V02 → V08
- [ ] DataLoss-Warnings anzeigen
- [ ] Dokumentation

**Akzeptanzkriterien:**
- Migration funktioniert
- Warnings sichtbar
- Dokumentiert

---

### Issue 107: Architecture Decision Records erstellen

**Labels:** `documentation`, `architecture`, `priority:medium`  
**Schätzung:** 2h

**Beschreibung:**  
Dokumentiere wichtige Architektur-Entscheidungen.

**Aufgaben:**
- [ ] ADR-Template erstellen
- [ ] ADRs schreiben:
  - ADR-001: Parser-Strategie (XmlReader)
  - ADR-002: Streaming mit IAsyncEnumerable
  - ADR-003: Versionierungs-Ansatz
  - ADR-004: Channel-basierte Pipeline
  - ADR-005: Schema-Caching
- [ ] In `/docs/adr/` ablegen

**Akzeptanzkriterien:**
- ADRs vollständig
- Entscheidungen begründet
- Zugänglich

---

## Phase 17: Code-Generierung (Optional)

### Issue 108: XSD→C# Generator evaluieren

**Labels:** `codegen`, `tooling`, `priority:low`  
**Schätzung:** 2h

**Beschreibung:**  
Evaluiere existierende Tools für Code-Generierung.

**Aufgaben:**
- [ ] XmlSchemaClassGenerator testen
- [ ] Output-Qualität bewerten:
  - Nullable Support
  - Collections (List vs Array)
  - Naming
- [ ] ISO20022-spezifische Probleme identifizieren
- [ ] Dokumentation der Evaluation

**Akzeptanzkriterien:**
- Evaluation dokumentiert
- Limitationen bekannt
- Empfehlung ausgesprochen

---

### Issue 109: Custom Code Generator erstellen

**Labels:** `codegen`, `tooling`, `priority:low`  
**Schätzung:** 4h

**Beschreibung:**  
Erstelle ggf. eigenen Code Generator.

**Aufgaben:**
- [ ] Source Generator Projekt
- [ ] XSD-Parsing
- [ ] C#-Code-Generierung:
  - Moderne Syntax
  - Nullable
  - Records wo sinnvoll
- [ ] Integration in Build
- [ ] Dokumentation

**Akzeptanzkriterien:**
- Generator funktioniert
- Output ist hochwertig
- Build-Integration

---

### Issue 110: Parser-Generator Konzept erstellen

**Labels:** `codegen`, `architecture`, `priority:low`  
**Schätzung:** 3-4h

**Beschreibung:**  
Konzeptioniere Generator für Parser-Code.

**Aufgaben:**
- [ ] Analyse: Was kann generiert werden?
- [ ] Konzept für XSD → Parser-Code
- [ ] Proof-of-Concept für einfache Messages
- [ ] Dokumentation der Grenzen
- [ ] ROI-Bewertung

**Akzeptanzkriterien:**
- Konzept dokumentiert
- PoC funktioniert
- ROI bewertet

---

## Zusammenfassung

| Phase | Thema | Issues | Geschätzte Zeit |
|-------|-------|--------|-----------------|
| 1 | Projekt-Setup | 5 | 6-9h |
| 2 | Core Domain Models | 8 | 14-19h |
| 3 | Core Parsing-Infrastruktur | 7 | 11-16h |
| 4 | PAIN Parser | 6 | 15-20h |
| 5 | PACS Parser | 6 | 14-18h |
| 6 | CAMT Parser | 8 | 16-22h |
| 7 | Weitere Business Areas | 7 | 13-18h |
| 8 | Streaming & Pipeline | 9 | 17-22h |
| 9 | Schema-Validierung | 6 | 13-17h |
| 10 | XML-Generierung | 7 | 16-22h |
| 11 | Transformation | 6 | 15-20h |
| 12 | Testing | 10 | 23-32h |
| 13 | Performance | 6 | 11-14h |
| 14 | Error Handling & Observability | 5 | 7-11h |
| 15 | DI & Konfiguration | 4 | 5-9h |
| 16 | Dokumentation | 7 | 15-20h |
| 17 | Code-Generierung (Optional) | 3 | 9-12h |
| **Gesamt** | | **110** | **~220-300h** |

---

## Empfohlene Sprint-Planung (MVP)

### Sprint 1: Foundation (Week 1-2)
- Issues 1-5 (Setup)
- Issues 6-13 (Core Domain)

### Sprint 2: Core Parsing (Week 3-4)
- Issues 14-20 (Parsing Infrastructure)
- Issues 21-26 (PAIN Parser)

### Sprint 3: CAMT & Statements (Week 5-6)
- Issues 33-40 (CAMT Parser)

### Sprint 4: Interbank (Week 7-8)
- Issues 27-32 (PACS Parser)

### Sprint 5: Pipeline (Week 9-10)
- Issues 48-56 (Streaming Pipeline)

### Sprint 6: Quality (Week 11-12)
- Issues 57-62 (Validation)
- Issues 76-85 (Testing)

### Sprint 7: Polish (Week 13-14)
- Issues 92-100 (Error Handling, DI)
- Issues 101-107 (Documentation)

---

## Label-Referenz

| Label | Beschreibung |
|-------|--------------|
| `priority:high` | Kritisch für MVP |
| `priority:medium` | Wichtig, aber nicht blockierend |
| `priority:low` | Nice-to-have |
| `pain` | PAIN Business Area |
| `pacs` | PACS Business Area |
| `camt` | CAMT Business Area |
| `acmt` | ACMT Business Area |
| `admi` | ADMI Business Area |
| `remt` | REMT Business Area |
| `head` | Business Application Header |
| `core` | Core-Infrastruktur |
| `domain` | Domain Models |
| `parsing` | Parsing-Funktionalität |
| `generation` | XML-Generierung |
| `validation` | Schema/Business-Validierung |
| `pipeline` | Channel-Pipeline |
| `streaming` | IAsyncEnumerable/Streaming |
| `transformation` | Versions-Transformation |
| `testing` | Tests |
| `performance` | Performance/Benchmarks |
| `documentation` | Dokumentation |
| `samples` | Beispiel-Projekte |
| `setup` | Projekt-Setup |
| `di` | Dependency Injection |
| `configuration` | Konfiguration |
| `observability` | Logging/Metrics/Tracing |
| `error-handling` | Exception-Handling |
| `tooling` | Entwickler-Tools |
| `codegen` | Code-Generierung |
| `ci-cd` | Build-Pipeline |
| `code-quality` | Code-Analyse |
