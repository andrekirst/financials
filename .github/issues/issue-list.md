# ISO 20022 Parser Library - Komplette Issue-Liste

> **ANLEITUNG:** Kopieren Sie Ihre komplette Issue-Liste hier hinein.
> Das Script `parse-and-create-issues.py` wird diese Datei parsen und
> alle Issues automatisch erstellen.

---

## Erwartetes Format

Das Script erwartet Issues im folgenden Format:

```markdown
### Issue 1: Solution-Struktur für ISO20022 anlegen

**Labels:** `setup`, `infrastructure`, `priority:high`
**Schätzung:** 1-2h

**Beschreibung:**
Erstelle eine .NET 8 Solution mit modularer Struktur für das ISO 20022 Parser-Projekt.

**Aufgaben:**
- [ ] Solution `ISO20022.sln` erstellen
- [ ] Projekt `ISO20022.Core` anlegen (Interfaces, Abstractions)
- [ ] Projekt `ISO20022.Domain` anlegen (Gemeinsame Models)
...

**Akzeptanzkriterien:**
- Solution baut erfolgreich
- Alle Projekte referenzieren korrekt
- Gemeinsame Build-Properties funktionieren

---

### Issue 2: Business-Area-Projekt-Template erstellen

**Labels:** `setup`, `documentation`, `priority:high`
**Schätzung:** 1-2h

...
```

---

## Hinweise

1. **Kopieren Sie Ihre komplette Issue-Liste hierhin** (alle 110 Issues)
2. **Stellen Sie sicher**, dass jedes Issue mit `### Issue X:` beginnt
3. **Labels** müssen in Backticks sein: `` `label1`, `label2` ``
4. **Schätzung** im Format: `X-Yh` oder `Xh`
5. **Trenner** zwischen Issues: `---` (optional aber empfohlen)

---

## TODO: Ihre Issue-Liste hier einfügen

Kopieren Sie hier Ihre komplette Markdown-Liste rein, beginnend mit:

```
### Issue 1: Solution-Struktur für ISO20022 anlegen
...
```

bis

```
### Issue 110: Parser-Generator Konzept erstellen
...
```

---

## Nach dem Einfügen

Führen Sie aus:

```bash
cd .github/issues

# Test-Modus (erstellt nur Markdown-Dateien)
python3 parse-and-create-issues.py

# Prüfen Sie die Dateien in individual/
ls -la individual/

# Produktiv-Modus (erstellt echte GitHub Issues)
# Editieren Sie parse-and-create-issues.py: DRY_RUN = False
python3 parse-and-create-issues.py
```
