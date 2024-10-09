using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ZusiMeterGaugesLib.CompoundGauges;

namespace ZusiMeter
{
  // 3.13 - 18.08.2024
  // Korrekturen:
  // - Fehler ´"Die Sequenz hat keine Elemente" behoben
  // - Anpassung an Zusi 3.5.x

  // 6.2 - 19.09.2024
  // Änderungen:
  // - Umstellung auf.net6
  // Korrekturen:
  // - Roadrunner piept wieder
  // Neu:
  // - Solldruck der Hauptluftleitung als horizontale und vertikale Anzeige.

  // 6.2.1 - 21.09.2024
  // Korrekturen:
  // - Titel und About-Inhalt stimmen wieder
  // - Vorschau geht im Konfigurator auch wieder
  // Neu:
  // - Solldruck der Hauptluftleitung in Manometer und Digitalanzeige

  // 6.3 - 23.09.2024
  // Korrekturen:
  // - Icons werden wieder richtig angezeigt
  // Neu:
  // - Im ZusiMeter Layout gibt es eine neuen Button "Zu ZUSI-Autostart hinzufügen" (hinter dem Button "Layout-Auswahl")
  //   Dieser Button ermöglicht es, das gerade aktive Layout automatisch mit einem Zugstart von ZUSI starten zu lassen.
  // - ZusiMeter wird nach dem ersten Start in das ZUSI-Menu Konfiguration eingetragen (Hinter ZUSI Display)
  //   Bei Auswahl des Menu-Punktes "ZusiMeter" wird automatisch das letzte aktive Layout geladen.
  //

  // 6.4 - 24.09.2024
  // Korrekturen:
  // - TextAnzeigen mit mehr als 4 Nachkommastellen werden jetzt vollständig angezeigt (inklusive "-")
  // Änderungen:
  // - Die ZusiMeterLayouts werden jetzt standardmäßig im öffentlichen Zusi-Datenverzeichnis gespeichert:
  //   %Daten%\_Tools\ZusiMeter\ZusiMeterLayouts
  //   Aus Kompatibilitätsgründen wird das bisherige Verzeichnis "Dokumente/ZusiMeterLayouts" weitervendet, wenn es existiert.

  // 6.4.1 - 25.09.2024
  // Korrekturen:
  // - Verbindung mit ZUSI geht wieder
  // Änderungen:
  // - Angezeigte Version von ZusiMeter 6.4.1

  // 6.5.0 - 29.09.2024
  // Änderungen:
  // - XCEED Extended WPF toolkit ersetzt durch DotNetProjects.Extended.Wpf.Toolkit (Lizenzänderung bei XCEED für freie Lizenz erlaubte nur noch 10 User)
  // - ZusiMeter: Hintergrund kann jetzt auf einen beliebiges Hintergrund, eine Farbe oder Transparent eingestellt werden.

  // 6.5.1 - 08.10.2024
  // Änderungen:
  // - ZusiMeterKonfigurator in ZusiMeter integriert
  // - Neues Hauptmenu - Datei, Einstellungen, Info
  // - Zusi-Verbindungseinstellungen über Einstellungsmenu - Einstellungen->Optionen->Verbindungen
  // - Dokumentation kann über Info->Dokumentation erreicht werden. Dokumentations ist in Arbeit...
  // - Kontext-Menu für Layout in der Layoutauswahl: "Layout anzeigen","Layout Bearbeiten","Layout zu ZUSI-Autostart hinzufügen"
  // - Mit Rechtsklick auf Verbidnungsicon rechts, kann in die Verbindugseinstellungen gewechselt werden.
  // - Aus Layoutanzeige oder Layoutbearbeiten zurück zur Layoutauswahl über Menü-Datei-Zurück zur Layoutauswahl.


}



