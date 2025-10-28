
# Fachliche Spezifikation – Zeiterfassungsanwendung
**Version 1.0**

---

## 1. Zielsetzung

Die Anwendung dient der **einfachen und persönlichen Erfassung von Arbeitszeit**. Im Zentrum steht ein **minimalistisches Bedienkonzept**, bei dem die Erfassung über **einen einzigen Start-/Stopp-Knopf** erfolgt. Die Lösung soll jederzeit erweiterbar bleiben, aber in der ersten Version möglichst unkompliziert und benutzerfreundlich funktionieren.

Jede Zeiterfassung ist einem Benutzer zugeordnet, sodass eine **benutzerspezifische Übersicht, Auswertung und Historie** gewährleistet ist.

---

## 2. Benutzerrollen

### Standardbenutzer
- Kann sich selbst registrieren
- Meldet sich mit Benutzername und Passwort an
- Hat Zugriff auf seine eigenen Zeiteinträge
- Kann seine eigenen Einträge anzeigen, bearbeiten, löschen und auswerten

---

## 3. Funktionale Anforderungen

### 3.1 Registrierung
- Benutzer gibt einen eindeutigen **Benutzernamen** und ein sicheres **Passwort** ein (mindestens 8 Zeichen)
- Optional kann eine **E-Mail-Adresse** angegeben werden
- Die Anwendung prüft, ob der Benutzername bereits vergeben ist
- Passwörter werden niemals im Klartext gespeichert, sondern als Hash persistiert

### 3.2 Login
- Benutzer meldet sich mit Benutzername und Passwort an
- Bei erfolgreicher Anmeldung wird eine Sitzung gestartet (z. B. über ein Token)
- Benutzer erhält Zugriff auf seine persönlichen Zeiterfassungsfunktionen

### 3.3 Logout
- Die Sitzung wird aktiv beendet
- Der Zugriff auf geschützte Bereiche ist danach nicht mehr möglich

### 3.4 Start-/Stopp-Zeiterfassung
- In der Hauptansicht befindet sich ein einzelner Button:
  - **Start**: Beginn einer neuen Zeiterfassung. Es wird der aktuelle Zeitpunkt gespeichert
  - **Stopp**: Beendet die laufende Erfassung. Endzeitpunkt wird gespeichert und Dauer berechnet
- Pro Benutzer kann **immer nur ein aktiver Zeitblock gleichzeitig** bestehen
- Bei einem Absturz oder Verbindungsabbruch muss der Zeitblock erhalten bleiben

### 3.5 Anzeige der Zeiteinträge
- Der Benutzer kann seine bisherigen Zeiteinträge in einer Liste einsehen
- Für jeden Eintrag werden angezeigt:
  - **Startzeit**
  - **Endzeit**
  - **Dauer**
  - Optional: **Notiz** und **Kategorie**
- Die Liste kann nach Zeitraum gefiltert werden (z. B. „Diese Woche“, „Benutzerdefiniert“)

### 3.6 Bearbeiten und Löschen von Einträgen
- Der Benutzer kann seine eigenen Einträge bearbeiten:
  - Start- und Endzeitpunkt
  - Kategorie
  - Notiz
- Nach der Bearbeitung wird die Dauer automatisch neu berechnet
- Der Benutzer kann Einträge löschen (mit Sicherheitsabfrage)

### 3.7 Summenansicht
- Die Anwendung zeigt die **aufsummierte Arbeitszeit** für bestimmte Zeiträume:
  - Heute
  - Diese Woche
  - Dieser Monat
  - Benutzerdefinierter Zeitraum (Start- und Enddatum)
- Optional:
  - Gruppierung nach Tagen mit Einzelwerten
  - Anzahl der Einträge im Zeitraum
  - Visualisierung (z. B. Balken pro Tag)
- Die Darstellung erfolgt im Format **Stunden:Minuten** (z. B. 07:45)

---

## 4. Geschäftsregeln

- Jeder Zeitblock muss einem registrierten Benutzer zugeordnet sein
- Nur abgeschlossene Zeitblöcke (mit Start- und Endzeit) werden in der Summenansicht berücksichtigt
- Startzeit darf nicht in der Zukunft liegen
- Endzeit muss nach Startzeit liegen
- Ein Benutzer darf nur **seine eigenen Daten** sehen und verändern
- Ein laufender Zeitblock muss **explizit gestoppt** werden, bevor ein neuer gestartet werden kann

---

## 5. Erweiterungsmöglichkeiten (optional)

Die folgende Funktionalität ist nicht Teil der ersten Version, aber für zukünftige Releases vorgesehen oder als Erweiterung denkbar:

- **Kategorisierung** der Zeiteinträge (z. B. nach Projekt, Typ, Kunde)
- **Notizfeld** je Eintrag (z. B. Beschreibung der Tätigkeit)
- **Export** der Daten als CSV oder PDF
- **Erinnerungsfunktion** (z. B. Push/Popup nach 8 Stunden Arbeit)
- **Kalender- oder Wochenansicht**
- **Mehrsprachigkeit / Lokalisierung**
- **Dark Mode / Theme-Unterstützung**
