#!/usr/bin/env python3
"""
ISO 20022 Parser Library - Issue Parser & Creator

Dieses Script parst die komplette Markdown-Issue-Liste und erstellt
automatisch alle GitHub Issues über die GitHub CLI.
"""

import re
import subprocess
import time
from pathlib import Path
from dataclasses import dataclass
from typing import List, Optional

# Konfiguration
REPO = "andrekirst/financials"
ISSUES_DIR = Path(__file__).parent / "individual"
MARKDOWN_FILE = Path(__file__).parent / "issue-list.md"  # Ihre komplette Liste
DRY_RUN = True  # Auf False setzen für echte Issue-Erstellung
DELAY_BETWEEN_ISSUES = 1  # Sekunden zwischen Issues (Rate Limiting)

@dataclass
class Issue:
    number: int
    title: str
    phase: str
    labels: List[str]
    estimate: str
    description: str
    tasks: List[str]
    acceptance_criteria: List[str]
    body: str  # Vollständiger Body

def parse_markdown_file(filepath: Path) -> List[Issue]:
    """Parse die Markdown-Datei und extrahiere alle Issues"""

    if not filepath.exists():
        print(f"❌ Datei nicht gefunden: {filepath}")
        print(f"   Bitte legen Sie Ihre Issue-Liste dort ab")
        return []

    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    issues = []

    # Regex-Pattern für Issue-Blöcke
    # Matcht: ### Issue X: Titel
    issue_pattern = r'### Issue (\d+): (.+?)\n\n\*\*Labels:\*\* `(.+?)`\s+\n\*\*Schätzung:\*\* (.+?)\n\n\*\*Beschreibung:\*\*\s+(.+?)(?=\n### Issue |\Z)'

    matches = re.finditer(issue_pattern, content, re.DOTALL)

    for match in matches:
        number = int(match.group(1))
        title = match.group(2).strip()
        labels_str = match.group(3).strip()
        estimate = match.group(4).strip()
        body_content = match.group(5).strip()

        # Parse Labels
        labels = [label.strip() for label in labels_str.split(',')]

        # Extrahiere Phase aus Content oder Labels
        phase = extract_phase(number)

        # Parse Tasks
        tasks = re.findall(r'- \[ \] (.+)', body_content)

        # Parse Acceptance Criteria
        acceptance_section = re.search(
            r'\*\*Akzeptanzkriterien:\*\*\s+(.+?)(?=\*\*|$)',
            body_content,
            re.DOTALL
        )
        acceptance_criteria = []
        if acceptance_section:
            acceptance_criteria = [
                line.strip('- ').strip()
                for line in acceptance_section.group(1).split('\n')
                if line.strip() and line.strip().startswith('-')
            ]

        # Erstelle vollständigen Body
        full_body = format_issue_body(
            body_content,
            tasks,
            acceptance_criteria,
            estimate
        )

        issue = Issue(
            number=number,
            title=title,
            phase=phase,
            labels=labels,
            estimate=estimate,
            description=body_content,
            tasks=tasks,
            acceptance_criteria=acceptance_criteria,
            body=full_body
        )

        issues.append(issue)

    return issues

def extract_phase(issue_number: int) -> str:
    """Bestimme Phase basierend auf Issue-Nummer"""
    phase_map = {
        (1, 5): "Phase 1: Foundation",
        (6, 13): "Phase 2: Core Domain",
        (14, 20): "Phase 3: Core Parsing",
        (21, 26): "Phase 4: PAIN Parser",
        (27, 32): "Phase 5: PACS Parser",
        (33, 40): "Phase 6: CAMT Parser",
        (41, 47): "Phase 7: Further Business Areas",
        (48, 56): "Phase 8: Streaming & Pipeline",
        (57, 62): "Phase 9: Schema Validation",
        (63, 69): "Phase 10: XML Generation",
        (70, 75): "Phase 11: Version Transformation",
        (76, 85): "Phase 12: Testing",
        (86, 91): "Phase 13: Performance",
        (92, 96): "Phase 14: Observability",
        (97, 100): "Phase 15: DI & Configuration",
        (101, 107): "Phase 16: Documentation",
        (108, 110): "Phase 17: Code Generation (Optional)",
    }

    for (start, end), phase in phase_map.items():
        if start <= issue_number <= end:
            return phase

    return "No Phase"

def format_issue_body(description: str, tasks: List[str],
                     acceptance_criteria: List[str], estimate: str) -> str:
    """Formatiere Issue-Body im GitHub-kompatiblen Markdown"""

    body_parts = []

    # Beschreibung (bereits formatiert aus Original)
    body_parts.append(description)

    # Schätzung am Ende
    body_parts.append(f"\n---\n\n**Schätzung:** {estimate}")

    return '\n\n'.join(body_parts)

def save_issue_markdown(issue: Issue):
    """Speichere Issue als Markdown-Datei"""
    ISSUES_DIR.mkdir(parents=True, exist_ok=True)

    filename = ISSUES_DIR / f"issue-{issue.number:03d}.md"

    content = f"""# Issue #{issue.number}: {issue.title}

**Labels:** {', '.join([f'`{label}`' for label in issue.labels])}
**Milestone:** {issue.phase}
**Estimate:** {issue.estimate}

---

{issue.body}
"""

    with open(filename, 'w', encoding='utf-8') as f:
        f.write(content)

    return filename

def create_github_issue(issue: Issue) -> bool:
    """Erstelle Issue über GitHub CLI"""

    if DRY_RUN:
        print(f"  [DRY RUN] Würde erstellen: #{issue.number} - {issue.title}")
        return True

    try:
        # Erstelle temporäre Datei für Body
        temp_file = ISSUES_DIR / f"temp-{issue.number}.md"
        with open(temp_file, 'w', encoding='utf-8') as f:
            f.write(issue.body)

        # Baue gh CLI Kommando
        cmd = [
            'gh', 'issue', 'create',
            '--repo', REPO,
            '--title', issue.title,
            '--body-file', str(temp_file),
            '--label', ','.join(issue.labels),
        ]

        # Füge Milestone hinzu wenn vorhanden
        if issue.phase != "No Phase":
            cmd.extend(['--milestone', issue.phase])

        # Führe Kommando aus
        result = subprocess.run(
            cmd,
            capture_output=True,
            text=True,
            timeout=30
        )

        # Lösche temporäre Datei
        temp_file.unlink(missing_ok=True)

        if result.returncode == 0:
            issue_url = result.stdout.strip()
            print(f"  ✅ Issue #{issue.number}: {issue.title}")
            print(f"     → {issue_url}")
            return True
        else:
            print(f"  ❌ Fehler bei Issue #{issue.number}: {result.stderr}")
            return False

    except subprocess.TimeoutExpired:
        print(f"  ⏱️ Timeout bei Issue #{issue.number}")
        return False
    except Exception as e:
        print(f"  ❌ Exception bei Issue #{issue.number}: {e}")
        return False

def check_prerequisites() -> bool:
    """Prüfe ob alle Voraussetzungen erfüllt sind"""

    # Prüfe ob Markdown-Datei existiert
    if not MARKDOWN_FILE.exists():
        print(f"❌ Issue-Liste nicht gefunden: {MARKDOWN_FILE}")
        print(f"\n💡 Bitte erstellen Sie die Datei mit Ihrer kompletten Issue-Liste")
        print(f"   oder passen Sie MARKDOWN_FILE im Script an")
        return False

    # Prüfe gh CLI
    try:
        result = subprocess.run(
            ['gh', '--version'],
            capture_output=True,
            check=True
        )
        print("✅ GitHub CLI gefunden")
    except (subprocess.CalledProcessError, FileNotFoundError):
        print("❌ GitHub CLI nicht gefunden")
        print("   Installation: https://cli.github.com/")
        return False

    # Prüfe Authentifizierung
    try:
        subprocess.run(
            ['gh', 'auth', 'status'],
            capture_output=True,
            check=True
        )
        print("✅ Bei GitHub angemeldet")
    except subprocess.CalledProcessError:
        print("❌ Nicht bei GitHub angemeldet")
        print("   Führe aus: gh auth login")
        return False

    return True

def main():
    print("=" * 70)
    print("🚀 ISO 20022 Issues Parser & Creator")
    print("=" * 70)
    print(f"📁 Repository: {REPO}")
    print(f"📄 Input: {MARKDOWN_FILE}")
    print(f"💾 Output: {ISSUES_DIR}")
    print(f"🔧 Modus: {'DRY RUN (nur Markdown)' if DRY_RUN else 'PRODUCTION (GitHub Issues)'}")
    print("=" * 70)
    print()

    # Voraussetzungen prüfen
    if not check_prerequisites():
        return 1

    print()
    print("📖 Parse Issue-Liste...")

    # Parse Markdown
    issues = parse_markdown_file(MARKDOWN_FILE)

    if not issues:
        print("❌ Keine Issues gefunden")
        print("\n💡 Hinweis: Das Script erwartet Issues im Format:")
        print("   ### Issue X: Titel")
        print("   **Labels:** `label1`, `label2`")
        print("   **Schätzung:** X-Yh")
        return 1

    print(f"✅ {len(issues)} Issues gefunden")
    print()

    # Erstelle Verzeichnis
    ISSUES_DIR.mkdir(parents=True, exist_ok=True)

    # Statistiken
    success_count = 0
    failed_count = 0

    # Verarbeite Issues
    print("📝 Erstelle Issues...")
    print()

    for i, issue in enumerate(issues, 1):
        print(f"[{i}/{len(issues)}] Issue #{issue.number}: {issue.title}")

        # Speichere Markdown
        markdown_file = save_issue_markdown(issue)
        print(f"  📄 Markdown: {markdown_file.name}")

        # Erstelle in GitHub
        if create_github_issue(issue):
            success_count += 1
        else:
            failed_count += 1

        # Rate Limiting
        if not DRY_RUN and i < len(issues):
            time.sleep(DELAY_BETWEEN_ISSUES)

        print()

    # Zusammenfassung
    print("=" * 70)
    print("✨ Fertig!")
    print("=" * 70)
    print(f"📊 Statistik:")
    print(f"   ✅ Erfolgreich: {success_count}")
    if failed_count > 0:
        print(f"   ❌ Fehlgeschlagen: {failed_count}")
    print(f"   📄 Markdown-Dateien: {ISSUES_DIR}")
    print()

    if DRY_RUN:
        print("💡 Nächste Schritte:")
        print("   1. Prüfen Sie die Markdown-Dateien in:", ISSUES_DIR)
        print("   2. Setzen Sie DRY_RUN = False im Script")
        print("   3. Führen Sie das Script erneut aus")
    else:
        print("🎉 Alle Issues wurden in GitHub erstellt!")
        print(f"   → https://github.com/{REPO}/issues")

    return 0 if failed_count == 0 else 1

if __name__ == "__main__":
    exit(main())
