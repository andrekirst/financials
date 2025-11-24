# ISO 20022 Parser Library - GitHub Issues

Diese Dateien enthalten alle geplanten Issues für das ISO 20022 Parser Library Projekt.

## Import-Methoden

### Methode 1: CSV Bulk Import (Empfohlen)

1. Gehe zu: `https://github.com/andrekirst/financials/issues/import`
2. Lade die Datei `issues-import.csv` hoch
3. GitHub erstellt automatisch alle Issues

### Methode 2: GitHub CLI (gh)

```bash
cd .github/issues
bash create-issues.sh
```

### Methode 3: Manuell

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
