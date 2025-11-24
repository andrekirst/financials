# ISO 20022 Parser Library - GitHub Issues

Diese Dateien enthalten alle geplanten Issues für das ISO 20022 Parser Library Projekt.

## 🚀 Schnellstart: Automatische Issue-Erstellung

### ⭐ Methode 1: Automatischer Parser (Empfohlen)

Das einfachste und schnellste Verfahren:

```bash
cd .github/issues

# 1. Kopiere deine komplette Issue-Liste in issue-list.md
nano issue-list.md  # oder bevorzugter Editor

# 2. Test-Modus (erstellt nur Markdown-Dateien)
python3 parse-and-create-issues.py

# 3. Prüfe die generierten Dateien
ls -la individual/

# 4. Produktiv-Modus (erstellt echte GitHub Issues)
# Editiere parse-and-create-issues.py: DRY_RUN = False
python3 parse-and-create-issues.py
```

**Vorteile:**
- ✅ Automatisches Parsen deiner Markdown-Liste
- ✅ Alle 110 Issues mit einem Befehl
- ✅ Test-Modus verfügbar
- ✅ Labels und Milestones automatisch
- ✅ Einzelne Markdown-Dateien als Backup

### Methode 2: CSV Bulk Import

1. Gehe zu: `https://github.com/andrekirst/financials/issues/import`
2. Lade die Datei `issues-import.csv` hoch
3. GitHub erstellt automatisch alle Issues

### Methode 3: GitHub CLI (gh) manuell

```bash
cd .github/issues
bash create-issues.sh
```

### Methode 4: Manuell

Nutze die einzelnen Markdown-Dateien im Ordner `individual/` als Vorlage.

## Struktur

```
.github/issues/
├── README.md                    # Diese Datei
├── issues-import.csv            # CSV für Bulk-Import
├── create-issues.sh             # Script für gh CLI
├── issue-template.md            # Master-Template
└── individual/                  # Einzelne Issue-Dateien
    ├── issue-001.md
    ├── issue-002.md
    └── ...
```

## Labels

Vor dem Import sollten folgende Labels in GitHub erstellt werden:

### Prioritäten
- `priority:high` (rot)
- `priority:medium` (gelb)
- `priority:low` (grün)

### Business Areas
- `pain`, `pacs`, `camt`, `acmt`, `admi`, `remt`, `head`

### Kategorien
- `core`, `domain`, `parsing`, `generation`, `validation`
- `pipeline`, `streaming`, `transformation`, `testing`
- `performance`, `documentation`, `samples`, `setup`
- `di`, `configuration`, `observability`, `error-handling`
- `tooling`, `codegen`, `ci-cd`, `code-quality`

## Geschätzte Timeline

- **Gesamt**: 110 Issues
- **Geschätzter Aufwand**: 220-300 Stunden
- **MVP (Sprint 1-4)**: ~60 Issues, 8-10 Wochen
- **Feature Complete**: 14 Wochen

## Phasen-Übersicht

| Phase | Issues | Aufwand |
|-------|--------|---------|
| Phase 1: Setup | 1-5 | 6-9h |
| Phase 2: Domain Models | 6-13 | 14-19h |
| Phase 3: Parsing Infra | 14-20 | 11-16h |
| Phase 4: PAIN Parser | 21-26 | 15-20h |
| Phase 5: PACS Parser | 27-32 | 14-18h |
| Phase 6: CAMT Parser | 33-40 | 16-22h |
| Phase 7: Weitere Areas | 41-47 | 13-18h |
| Phase 8: Pipeline | 48-56 | 17-22h |
| Phase 9: Validation | 57-62 | 13-17h |
| Phase 10: Generation | 63-69 | 16-22h |
| Phase 11: Transformation | 70-75 | 15-20h |
| Phase 12: Testing | 76-85 | 23-32h |
| Phase 13: Performance | 86-91 | 11-14h |
| Phase 14: Observability | 92-96 | 7-11h |
| Phase 15: DI/Config | 97-100 | 5-9h |
| Phase 16: Docs | 101-107 | 15-20h |
| Phase 17: Codegen | 108-110 | 9-12h |
