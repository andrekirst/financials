# Anleitung: GitHub Issues erstellen

## Übersicht

Sie haben 3 Möglichkeiten, die 110 Issues aus Ihrer Liste in GitHub zu importieren:

### ⭐ Methode 1: Automatisches Python-Script (Empfohlen)

Das Python-Script `generate-issues.py` muss noch mit Ihren vollständigen Issue-Daten gefüllt werden.

**Vorbereitung:**

1. Öffnen Sie `generate-issues.py`
2. Erweitern Sie das `ISSUES`-Array mit allen 110 Issues aus Ihrer Markdown-Liste
3. Das Format ist bereits vorgegeben

**Ausführung:**

```bash
cd .github/issues

# Test-Modus (erstellt nur Markdown-Dateien)
python3 generate-issues.py  # DRY_RUN = True

# Produktiv-Modus (erstellt echte GitHub Issues)
# Editiere generate-issues.py: DRY_RUN = False
python3 generate-issues.py
```

**Vorteile:**
- ✅ Automatisch alle 110 Issues
- ✅ Konsistente Formatierung
- ✅ Labels und Milestones automatisch
- ✅ Test-Modus verfügbar

### 📝 Methode 2: Bash-Script mit GitHub CLI

Das Bash-Script zeigt Ihnen das Muster für die ersten 3 Issues.

**Vorbereitung:**

```bash
# GitHub CLI installieren (falls nicht vorhanden)
# macOS
brew install gh

# Linux
sudo apt install gh

# Anmelden
gh auth login
```

**Ausführung:**

```bash
cd .github/issues
bash create-issues.sh
```

**Um alle Issues zu erstellen:**
Erweitern Sie `create-issues.sh` mit allen Issues nach dem gezeigten Muster.

### 🖐️ Methode 3: Manuell kopieren

Falls Sie Issues manuell erstellen möchten:

1. Gehe zu: https://github.com/andrekirst/financials/issues/new
2. Kopiere Titel und Beschreibung aus der Original-Markdown-Liste
3. Füge Labels und Milestone hinzu
4. Erstelle Issue
5. Wiederhole für alle 110 Issues 😅

## Labels erstellen

Vor dem Import sollten Sie folgende Labels in GitHub erstellen:

**Repository Settings → Labels → New label**

### Prioritäten
| Name | Beschreibung | Farbe |
|------|--------------|-------|
| `priority:high` | Kritisch für MVP | #d73a4a (rot) |
| `priority:medium` | Wichtig, aber nicht blockierend | #fbca04 (gelb) |
| `priority:low` | Nice-to-have | #0e8a16 (grün) |

### Business Areas
| Name | Beschreibung | Farbe |
|------|--------------|-------|
| `pain` | PAIN Business Area | #1d76db |
| `pacs` | PACS Business Area | #1d76db |
| `camt` | CAMT Business Area | #1d76db |
| `acmt` | ACMT Business Area | #1d76db |
| `admi` | ADMI Business Area | #1d76db |
| `remt` | REMT Business Area | #1d76db |
| `head` | Business Application Header | #1d76db |

### Kategorien
| Name | Beschreibung | Farbe |
|------|--------------|-------|
| `setup` | Projekt-Setup | #0075ca |
| `core` | Core-Infrastruktur | #0075ca |
| `domain` | Domain Models | #0075ca |
| `parsing` | Parsing-Funktionalität | #0075ca |
| `generation` | XML-Generierung | #0075ca |
| `validation` | Validierung | #0075ca |
| `pipeline` | Channel-Pipeline | #0075ca |
| `streaming` | IAsyncEnumerable/Streaming | #0075ca |
| `transformation` | Versions-Transformation | #0075ca |
| `testing` | Tests | #0075ca |
| `performance` | Performance/Benchmarks | #0075ca |
| `documentation` | Dokumentation | #0075ca |
| `samples` | Beispiel-Projekte | #0075ca |
| `di` | Dependency Injection | #0075ca |
| `configuration` | Konfiguration | #0075ca |
| `observability` | Logging/Metrics/Tracing | #0075ca |
| `error-handling` | Exception-Handling | #0075ca |
| `tooling` | Entwickler-Tools | #0075ca |
| `codegen` | Code-Generierung | #0075ca |
| `ci-cd` | Build-Pipeline | #0075ca |
| `code-quality` | Code-Analyse | #0075ca |

## Milestones erstellen

Erstellen Sie folgende Milestones:

**Repository Settings → Milestones → New milestone**

1. **Phase 1: Foundation** (Issues 1-5)
   - Due date: 2 Wochen

2. **Phase 2: Core Domain** (Issues 6-13)
   - Due date: 2 Wochen

3. **Phase 3: Core Parsing** (Issues 14-20)
   - Due date: 2 Wochen

4. **Phase 4: PAIN Parser** (Issues 21-26)
   - Due date: 2 Wochen

5. **Phase 5: PACS Parser** (Issues 27-32)
   - Due date: 2 Wochen

6. **Phase 6: CAMT Parser** (Issues 33-40)
   - Due date: 2 Wochen

...und so weiter für alle Phasen.

## Script anpassen

### Python-Script erweitern

Um alle 110 Issues hinzuzufügen, editieren Sie `generate-issues.py`:

```python
ISSUES = [
    {
        "number": 1,
        "title": "Solution-Struktur für ISO20022 anlegen",
        "labels": ["setup", "infrastructure", "priority:high"],
        "milestone": "Phase 1: Foundation",
        "estimate": "1-2h",
        "body": """..."""
    },
    {
        "number": 2,
        "title": "Business-Area-Projekt-Template erstellen",
        ...
    },
    # Fügen Sie alle 110 Issues hier ein
]
```

### Bash-Script erweitern

Für das Bash-Script kopieren Sie das Muster:

```bash
gh issue create \
  --repo "$REPO" \
  --title "Ihr Issue-Titel" \
  --body-file - \
  --label "label1,label2,priority:high" \
  --milestone "Phase X: Name" << 'EOF'
[Issue-Beschreibung hier]
EOF
```

## Tipps

1. **Test erst lokal:** Nutzen Sie DRY_RUN beim Python-Script
2. **Rate Limiting:** GitHub hat API-Limits, ggf. Pausen einbauen
3. **Batch-Verarbeitung:** Erstellen Sie Issues in mehreren Durchläufen
4. **Issue-Templates:** Sie können auch GitHub Issue Templates erstellen

## Hilfe

Bei Problemen:

```bash
# GitHub CLI Version prüfen
gh --version

# Authentifizierung prüfen
gh auth status

# Hilfe anzeigen
gh issue create --help
```

## Nächste Schritte

1. ✅ Labels in GitHub erstellen
2. ✅ Milestones in GitHub erstellen
3. ✅ Script mit allen Issues füllen
4. ✅ Test-Durchlauf (DRY_RUN)
5. ✅ Produktiv-Durchlauf
6. 🎉 Issues in GitHub!
