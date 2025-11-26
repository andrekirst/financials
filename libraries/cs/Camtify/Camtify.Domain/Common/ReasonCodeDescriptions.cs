namespace Camtify.Domain.Common;

/// <summary>
/// German descriptions for ISO 20022 reason codes.
/// </summary>
/// <remarks>
/// Provides human-readable German translations for reason codes used in payment messages.
/// </remarks>
public static class ReasonCodeDescriptions
{
    private static readonly Dictionary<string, string> Descriptions = new()
    {
        // Account related codes (AC01-AC14)
        [ReasonCodes.AC01] = "Kontonummer ungültig oder fehlt",
        [ReasonCodes.AC04] = "Belastete Kontonummer ungültig oder fehlt",
        [ReasonCodes.AC06] = "Konto geschlossen",
        [ReasonCodes.AC13] = "Konto gesperrt",
        [ReasonCodes.AC14] = "Begünstigte Kontonummer ungültig",

        // Amount related codes (AM01-AM10)
        [ReasonCodes.AM01] = "Betrag ist null",
        [ReasonCodes.AM02] = "Betrag nicht zulässig",
        [ReasonCodes.AM04] = "Unzureichende Deckung",
        [ReasonCodes.AM05] = "Doppelte Überweisung",
        [ReasonCodes.AM09] = "Betrag zu niedrig",
        [ReasonCodes.AM10] = "Kontrollsumme ungültig",

        // Bank Entity codes (BE01-BE07)
        [ReasonCodes.BE01] = "Kennung stimmt nicht mit Konto überein",
        [ReasonCodes.BE04] = "Begünstigter unbekannt",
        [ReasonCodes.BE05] = "Adresse des Begünstigten fehlt oder fehlerhaft",
        [ReasonCodes.BE06] = "Begünstigter unbekannt",
        [ReasonCodes.BE07] = "Adresse des Schuldners fehlt oder fehlerhaft",

        // Date/Time codes (DT01-DT05)
        [ReasonCodes.DT01] = "Ungültiges oder fehlendes Datum",
        [ReasonCodes.DT02] = "Cut-Off-Zeit überschritten",
        [ReasonCodes.DT03] = "Ungültiges Erstellungsdatum",
        [ReasonCodes.DT04] = "Zukünftiges Datum nicht unterstützt",
        [ReasonCodes.DT05] = "Ungültiger Bank-Operationscode",

        // Financial Flow codes (FF01-FF07)
        [ReasonCodes.FF01] = "Ungültiges Dateiformat",
        [ReasonCodes.FF02] = "Syntaxfehler",
        [ReasonCodes.FF03] = "Ungültige Nachrichtenkennung",
        [ReasonCodes.FF04] = "Ungültige Zahlungsartinformation",
        [ReasonCodes.FF05] = "Ungültiger Service-Level",
        [ReasonCodes.FF06] = "Ungültiges lokales Instrument",
        [ReasonCodes.FF07] = "Ungültiger Verwendungszweck",

        // Mandate codes (MD01-MD07)
        [ReasonCodes.MD01] = "Mandat nicht gefunden",
        [ReasonCodes.MD02] = "Pflichtangaben im Mandat fehlen",
        [ReasonCodes.MD03] = "Ungültiges Datenformat im Mandat",
        [ReasonCodes.MD06] = "Erstattungsanforderung durch Endkunden",
        [ReasonCodes.MD07] = "Endkunde verstorben",

        // Regulatory Reporting codes (RR01-RR04)
        [ReasonCodes.RR01] = "Meldepflichtige Informationen fehlen",
        [ReasonCodes.RR02] = "Meldepflichtige Informationen ungültig",
        [ReasonCodes.RR03] = "Name des Begünstigten fehlt",
        [ReasonCodes.RR04] = "Regulatorischer Grund",

        // Timeout codes
        [ReasonCodes.TM01] = "Zeitlimit überschritten",

        // Technical codes (TS01-TS02)
        [ReasonCodes.TS01] = "Technisches Problem",
        [ReasonCodes.TS02] = "System vorübergehend nicht verfügbar",

        // Special codes
        [ReasonCodes.NARR] = "Freitext-Begründung",
        [ReasonCodes.FOCR] = "Folgt Stornierungsanforderung",
        [ReasonCodes.FRAD] = "Betrügerischen Ursprungs"
    };

    /// <summary>
    /// Gets the German description for a reason code.
    /// </summary>
    /// <param name="reasonCode">The ISO 20022 reason code.</param>
    /// <returns>The German description, or the reason code itself if no description is available.</returns>
    public static string GetDescription(string reasonCode)
    {
        if (string.IsNullOrWhiteSpace(reasonCode))
        {
            return string.Empty;
        }

        return Descriptions.TryGetValue(reasonCode, out var description) 
            ? description 
            : reasonCode;
    }

    /// <summary>
    /// Determines whether a description exists for the specified reason code.
    /// </summary>
    /// <param name="reasonCode">The ISO 20022 reason code.</param>
    /// <returns>True if a description exists; otherwise, false.</returns>
    public static bool HasDescription(string reasonCode)
    {
        return !string.IsNullOrWhiteSpace(reasonCode) && Descriptions.ContainsKey(reasonCode);
    }

    /// <summary>
    /// Gets all available reason codes.
    /// </summary>
    /// <returns>A collection of all available reason codes.</returns>
    public static IReadOnlyCollection<string> GetAllReasonCodes()
    {
        return Descriptions.Keys.ToList().AsReadOnly();
    }
}
