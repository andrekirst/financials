#!/usr/bin/env python3
"""
ISO 20022 Parser Library - GitHub Issues Generator

Dieses Script konvertiert die Markdown-Issue-Liste in einzelne GitHub Issues.
"""

import re
import os
import subprocess
from pathlib import Path

# Konfiguration
REPO = "andrekirst/financials"
ISSUES_DIR = Path(__file__).parent / "individual"
DRY_RUN = False  # Auf True setzen für Test ohne tatsächliche Issue-Erstellung

# Issue-Daten (gekürzte Version - vollständige Daten aus Ihrer Liste)
ISSUES = [
    {
        "number": 1,
        "title": "Solution-Struktur für ISO20022 anlegen",
        "labels": ["setup", "infrastructure", "priority:high"],
        "milestone": "Phase 1: Foundation",
        "estimate": "1-2h",
        "body": """## Beschreibung

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
{estimate}
"""
    },
    # Weitere Issues würden hier aus Ihrer originalen Liste extrahiert...
]

def create_issues_directory():
    """Erstelle Verzeichnisstruktur"""
    ISSUES_DIR.mkdir(parents=True, exist_ok=True)
    print(f"✅ Verzeichnis erstellt: {ISSUES_DIR}")

def save_issue_markdown(issue):
    """Speichere Issue als Markdown-Datei"""
    filename = ISSUES_DIR / f"issue-{issue['number']:03d}.md"

    content = f"""# {issue['title']}

**Labels:** {', '.join(issue['labels'])}
**Milestone:** {issue['milestone']}
**Estimate:** {issue['estimate']}

---

{issue['body'].format(estimate=issue['estimate'])}
"""

    with open(filename, 'w', encoding='utf-8') as f:
        f.write(content)

    return filename

def create_github_issue(issue):
    """Erstelle Issue über GitHub CLI"""
    if DRY_RUN:
        print(f"  [DRY RUN] Würde Issue erstellen: {issue['title']}")
        return

    try:
        # Erstelle temporäre Datei für Body
        temp_file = ISSUES_DIR / f"temp-{issue['number']}.md"
        with open(temp_file, 'w', encoding='utf-8') as f:
            f.write(issue['body'].format(estimate=issue['estimate']))

        # Erstelle Issue mit gh CLI
        cmd = [
            'gh', 'issue', 'create',
            '--repo', REPO,
            '--title', issue['title'],
            '--body-file', str(temp_file),
            '--label', ','.join(issue['labels']),
        ]

        if issue.get('milestone'):
            cmd.extend(['--milestone', issue['milestone']])

        result = subprocess.run(cmd, capture_output=True, text=True)

        if result.returncode == 0:
            print(f"  ✅ Issue #{issue['number']}: {issue['title']}")
        else:
            print(f"  ❌ Fehler bei Issue #{issue['number']}: {result.stderr}")

        # Lösche temporäre Datei
        temp_file.unlink(missing_ok=True)

    except Exception as e:
        print(f"  ❌ Exception bei Issue #{issue['number']}: {e}")

def main():
    print("🚀 ISO 20022 Issues Generator")
    print(f"📁 Repository: {REPO}")
    print(f"📝 Anzahl Issues: {len(ISSUES)}")
    print(f"🔧 Modus: {'DRY RUN' if DRY_RUN else 'PRODUCTION'}")
    print()

    # Prüfe ob gh CLI verfügbar ist
    try:
        subprocess.run(['gh', '--version'], capture_output=True, check=True)
        print("✅ GitHub CLI gefunden")
    except (subprocess.CalledProcessError, FileNotFoundError):
        print("❌ GitHub CLI nicht gefunden")
        print("   Installation: https://cli.github.com/")
        return

    # Prüfe Authentifizierung
    try:
        subprocess.run(['gh', 'auth', 'status'], capture_output=True, check=True)
        print("✅ Bei GitHub angemeldet")
    except subprocess.CalledProcessError:
        print("❌ Nicht bei GitHub angemeldet")
        print("   Führe aus: gh auth login")
        return

    print()

    # Erstelle Verzeichnis
    create_issues_directory()

    # Verarbeite Issues
    print("\n📋 Erstelle Issues...")
    for issue in ISSUES:
        # Speichere als Markdown
        filename = save_issue_markdown(issue)
        print(f"  📄 Markdown: {filename.name}")

        # Erstelle in GitHub
        if not DRY_RUN:
            create_github_issue(issue)

    print("\n✨ Fertig!")
    print(f"\n💡 Tipp: Setze DRY_RUN = True zum Testen")

if __name__ == "__main__":
    main()
