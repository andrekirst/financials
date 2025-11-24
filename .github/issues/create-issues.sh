#!/bin/bash

# ISO 20022 Parser Library - GitHub Issues Creator
# Dieses Script erstellt alle geplanten Issues über die GitHub CLI

set -e

REPO="andrekirst/financials"

echo "🚀 Erstelle Issues für $REPO..."
echo "📋 Insgesamt 110 Issues werden erstellt"
echo ""

# Prüfe ob gh CLI installiert ist
if ! command -v gh &> /dev/null; then
    echo "❌ GitHub CLI (gh) ist nicht installiert"
    echo "   Installation: https://cli.github.com/"
    exit 1
fi

# Prüfe Authentifizierung
if ! gh auth status &> /dev/null; then
    echo "❌ Nicht bei GitHub angemeldet"
    echo "   Führe aus: gh auth login"
    exit 1
fi

echo "✅ GitHub CLI ist bereit"
echo ""

# Phase 1: Setup & Infrastructure (Issues 1-5)
echo "📦 Phase 1: Setup & Infrastructure"

gh issue create \
  --repo "$REPO" \
  --title "Solution-Struktur für ISO20022 anlegen" \
  --body-file - \
  --label "setup,infrastructure,priority:high" \
  --milestone "Phase 1: Foundation" << 'EOF'
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
- [ ] `Directory.Build.props` mit gemeinsamen Settings konfigurieren
- [ ] Struktur für separate NuGet-Pakete vorbereiten

## Akzeptanzkriterien

- Solution baut erfolgreich
- Alle Projekte referenzieren korrekt
- Gemeinsame Build-Properties funktionieren

## Schätzung
1-2h
EOF

echo "✅ Issue 1 erstellt"

gh issue create \
  --repo "$REPO" \
  --title "Business-Area-Projekt-Template erstellen" \
  --body-file - \
  --label "setup,documentation,priority:high" \
  --milestone "Phase 1: Foundation" << 'EOF'
## Beschreibung

Erstelle ein Template und Konventionen für Business-Area-spezifische Projekte.

## Aufgaben

- [ ] Template-Struktur definieren für `ISO20022.{Area}` Projekte
- [ ] Ordnerstruktur festlegen
- [ ] Namespace-Konventionen dokumentieren
- [ ] Dateibenennung-Konventionen definieren
- [ ] CONTRIBUTING.md mit Entwickler-Guidelines erstellen
- [ ] Beispiel-Projekt `ISO20022.Pain` als Referenz anlegen

## Akzeptanzkriterien

- Template ist dokumentiert
- Beispiel-Projekt folgt Template
- CONTRIBUTING.md ist vollständig

## Schätzung
1-2h
EOF

echo "✅ Issue 2 erstellt"

gh issue create \
  --repo "$REPO" \
  --title "NuGet Central Package Management" \
  --body-file - \
  --label "setup,dependencies,priority:medium" \
  --milestone "Phase 1: Foundation" << 'EOF'
## Beschreibung

Konfiguriere zentrales Package Management für konsistente Dependency-Versionen.

## Aufgaben

- [ ] `Directory.Packages.props` erstellen
- [ ] Core-Dependencies hinzufügen
- [ ] Test-Dependencies hinzufügen
- [ ] Package-Struktur für NuGet-Veröffentlichung planen
- [ ] `nuget.config` für Package-Sources konfigurieren

## Akzeptanzkriterien

- Alle Projekte nutzen zentrale Versionen
- Keine Version-Konflikte
- Dokumentation der Package-Strategie

## Schätzung
1h
EOF

echo "✅ Issue 3 erstellt"

# Hinweis: Weitere Issues würden hier folgen...
echo ""
echo "📝 Hinweis: Dies ist eine gekürzte Version des Scripts"
echo "   Die vollständige Version würde alle 110 Issues erstellen"
echo ""
echo "✨ Demo abgeschlossen!"
