# Implementierungsplan Backend MVP – Persönliche Zeiterfassung

## Ziel
Schrittweise Umsetzung des Backend-MVP auf Basis der fachlichen, technischen und Test-Spezifikation.

## Leitplanken (verbindlich)
- MVP enthält Notiz und Kategorie.
- Start/Stop idempotent.
- Start bei aktivem Eintrag: 409.
- Stop ohne aktiven Eintrag: 409.
- Zeiträume und Summen in UTC.
- Wochenstart: Montag (ISO-8601).
- Filter von/to: inklusive.
- Überlappende Einträge verhindern.
- Serverzeit ist führend.
- Dauer wird berechnet, nicht als führende Quelle gespeichert.
- Mehrgeräte-Sessions erlaubt.
- Logout beendet nur aktuelles Gerät.
- Soft Delete für Zeiteinträge.
- Problem Details nach RFC 7807.
- Keine API-Versionierung im MVP.
- Rate limiting: out of scope für MVP.
- Integrationstests: nur PRs und Main-Branch.
- Coverage-Ziel Domain/Application: verbindlich.

## Offene Architekturentscheidungen vor Start
## Architekturentscheidungen (final bestätigt)
- [x] Refresh-Token-Strategie: Rotation + Reuse Detection
- [x] Passwortregeln: Mindestens 12 Zeichen, Blocklist kompromittierter Passwörter

---

## Fortschrittsstatus
- Gesamtfortschritt: 10%
- Aktiver Meilenstein: 1 (Projektgrundlage und Solution-Struktur)
- Nächster Schritt: 1.2 Grundkonfiguration
### 1.1 Solution und Projekte anlegen
- [x] Root-Struktur gemäß Spezifikation prüfen/erstellen (backend + Teilprojekte).
- [x] Solution-Datei erstellen.
- [x] Projekt Zeiterfassung.API erstellen.
- [x] Projekt Zeiterfassung.Application erstellen.
- [x] Projekt Zeiterfassung.Domain erstellen.
- [x] Projekt Zeiterfassung.Infrastructure erstellen.
- [x] Projekt-Referenzen korrekt verdrahten.

### 1.2 Grundkonfiguration
****- [x] Gemeinsame Build-Einstellungen definieren.
- [x] Nullable aktivieren.
- [x] Warnings as errors für produktive Projekte aktivieren.
- [x] Basiskonfiguration für appsettings erstellen.
- [x] Entwicklungs-Konfiguration ergänzen.

### 1.3 Docker-Basis
- [ ] Dockerfile für API-Projekt anlegen.
- [ ] docker-compose mit API + PostgreSQL anlegen.
- [ ] Persistentes DB-Volume konfigurieren.
- [ ] Environment-Variablen für DB/JWT dokumentieren.
- [ ] Lokalen Start via docker-compose validieren.

### 1.4 Health und Observability-Basis
- [ ] Health-Endpoint bereitstellen.
- [ ] DB-Healthcheck integrieren.
- [ ] Logging-Grundkonfiguration (Serilog oder Microsoft Logging) setzen.

---

## Meilenstein 2 – Domain-Modell und Persistenz

### 2.1 Domain-Entitäten und Regeln
- [ ] Entität User modellieren.
- [ ] Entität TimeEntry modellieren (inkl. Soft-Delete-Felder).
- [ ] Entität RefreshToken modellieren.
- [ ] Invarianten für TimeEntry implementieren (Ende nach Start, kein Start in Zukunft etc.).
- [ ] Regel zur Verhinderung überlappender Einträge modellieren.

### 2.2 EF Core und DbContext
- [ ] DbContext mit Sets für User, TimeEntries, RefreshTokens erstellen.
- [ ] Fluent-Konfigurationen für alle Entitäten anlegen.
- [ ] Unique-Constraints definieren (z. B. Username, Refresh-Token).
- [ ] Indizes für typische Queries definieren (user_id, start_time, end_time, deleted_at).
- [ ] Soft-Delete-Filter für TimeEntries konfigurieren.

### 2.3 Migrationen
- [ ] Initiale Migration erzeugen.
- [ ] UTC-konforme Spaltentypen prüfen.
- [ ] Migration lokal gegen PostgreSQL ausführen.
- [ ] Rollback-Szenario einmal validieren.

### 2.4 Repositories / Datenzugriff
- [ ] Repository-Interfaces in Application definieren.
- [ ] Repository-Implementierungen in Infrastructure erstellen.
- [ ] Queries für aktiven Zeiteintrag optimieren.
- [ ] Query für Überlappungsprüfung implementieren.

---

## Meilenstein 3 – Authentifizierung und Session-Management

### 3.1 Registrierung
- [ ] DTOs für Register-Request/Response anlegen.
- [ ] Username- und Passwortvalidierung implementieren.
- [ ] E-Mail optional validieren.
- [ ] Passwort-Hashing integrieren.
- [ ] Register-Use-Case implementieren.

### 3.2 Login
- [ ] DTOs für Login-Request/Response anlegen.
- [ ] Credential-Prüfung implementieren.
- [ ] Access-Token-Erzeugung implementieren.
- [ ] Refresh-Token-Erzeugung implementieren.
- [ ] Persistenz des Refresh-Tokens pro Session/Gerät umsetzen.

### 3.3 Refresh und Logout
- [ ] Refresh-Endpoint mit gewählter Token-Strategie implementieren.
- [ ] Token-Rotation implementieren (falls entschieden).
- [ ] Reuse-Detection implementieren (falls entschieden).
- [ ] Logout für aktuelle Session implementieren.
- [ ] Invalidierte/abgelaufene Tokens zuverlässig behandeln.

### 3.4 Auth-Infrastruktur
- [ ] JWT-Bearer-Auth in API konfigurieren.
- [ ] Claims-Design festlegen (sub, username, session id).
- [ ] Autorisierungsrichtlinien für geschützte Endpunkte definieren.
- [ ] Endpoint GET /auth/me implementieren.

---

## Meilenstein 4 – Zeitbuchung Start/Stop und CRUD

### 4.1 Start/Stop-Fachlogik
- [ ] Use Case Start implementieren (idempotent).
- [ ] Use Case Stop implementieren (idempotent).
- [ ] 409-Fehlerfälle exakt umsetzen.
- [ ] Serverzeit als Quelle für Zeitstempel erzwingen.
- [ ] Nur ein aktiver Eintrag pro Nutzer technisch absichern.

### 4.2 Active und List
- [ ] Endpoint GET /time/active implementieren.
- [ ] Endpoint GET /time/list mit Zeitraumfilter implementieren.
- [ ] Inklusive Filtergrenzen korrekt umsetzen.
- [ ] Sortierung und Paging-Strategie festlegen und umsetzen.

### 4.3 Update und Delete
- [ ] Endpoint PUT /time/{id} implementieren.
- [ ] Validierung Start/Ende/Future/Overlap für Updates implementieren.
- [ ] Dauer bei Änderungen neu berechnen.
- [ ] Endpoint DELETE /time/{id} als Soft Delete implementieren.
- [ ] Sicherheitsprüfung: nur eigene Daten veränderbar.

### 4.4 Idempotenz und Korrektheit
- [ ] Idempotenz-Key-Strategie für Start/Stop festlegen (falls benötigt).
- [ ] Nebenläufigkeit absichern (z. B. DB-Transaktion/Row Lock/Unique Constraint).
- [ ] Race-Condition-Testszenarien definieren.

---

## Meilenstein 5 – Summenansicht und Auswertung

### 5.1 Summary-Logik
- [ ] Endpoint GET /time/summary implementieren.
- [ ] Summenberechnung nur für abgeschlossene Einträge.
- [ ] UTC-Interpretation aller Zeitfenster sicherstellen.
- [ ] Woche nach ISO (Montag) korrekt berechnen.
- [ ] Ausgabeformat für aggregierte Dauer festlegen.

### 5.2 Erweiterte Aggregationen im MVP
- [ ] Anzahl Einträge im Zeitraum zurückgeben.
- [ ] Tagesgruppierung als optionales Response-Feld implementieren.
- [ ] Kategorie-/Notiz-Felder in Response-Modellen konsistent abbilden.

---

## Meilenstein 6 – API-Qualität, Fehler und Sicherheit

### 6.1 Problem Details
- [ ] Zentrale Exception-Middleware erstellen.
- [ ] Fehler auf ProblemDetails nach RFC 7807 mappen.
- [ ] Einheitliche Fehlercodes/Typen definieren.
- [ ] Feldvalidierungsfehler standardisiert zurückgeben.

### 6.2 Zugriffsschutz
- [ ] Ownership-Checks in allen Time-Endpunkten prüfen.
- [ ] 401/403/404/409-Semantik konsistent umsetzen.
- [ ] Sensitive Daten in Responses und Logs ausblenden.

### 6.3 Logging
- [ ] Strukturierte Logs für Auth- und Time-Aktionen ergänzen.
- [ ] Fehlerlogs mit Korrelations-ID anreichern.
- [ ] Log-Level je Umgebung konfigurieren.

---

## Meilenstein 7 – Unit Tests (Domain/Application)

### 7.1 Testprojekte aufsetzen
- [ ] Zeiterfassung.Domain.UnitTests erstellen.
- [ ] Zeiterfassung.Application.UnitTests erstellen.
- [ ] Testpakete (xUnit, NSubstitute) einbinden.

### 7.2 Domain-Testabdeckung
- [ ] Tests für Zeitinvarianten erstellen.
- [ ] Tests für Dauerberechnung erstellen.
- [ ] Tests für Overlap-Regeln erstellen.
- [ ] Tests für Soft-Delete-relevante Domänenlogik erstellen.

### 7.3 Application-Testabdeckung
- [ ] Tests für Register/Login/Refresh/Logout-Use-Cases erstellen.
- [ ] Tests für Start/Stop-Idempotenz erstellen.
- [ ] Tests für 409-Konfliktfälle erstellen.
- [ ] Tests für List/Summary-Filterregeln erstellen.

### 7.4 Coverage-Gate
- [ ] Coverage-Tool integrieren (z. B. coverlet).
- [ ] Verbindliches Coverage-Gate konfigurieren.
- [ ] Build soll bei Unterschreitung fehlschlagen.

---

## Meilenstein 8 – Integrationstests (API + PostgreSQL)

### 8.1 Testinfrastruktur
- [ ] Zeiterfassung.API.IntegrationTests erstellen.
- [ ] Testcontainers für PostgreSQL integrieren.
- [ ] Migrationen beim Teststart automatisch ausführen.
- [ ] Isolierte Testdaten pro Testfall sicherstellen.

### 8.2 Auth-Endpunkte testen
- [ ] Register happy path + Fehlerfälle testen.
- [ ] Login happy path + Fehlerfälle testen.
- [ ] Refresh-Flow gemäß finaler Strategie testen.
- [ ] Logout aktueller Session testen.
- [ ] GET /auth/me testen.

### 8.3 Time-Endpunkte testen
- [ ] Start/Stop happy path testen.
- [ ] Idempotenz für Start/Stop testen.
- [ ] 409-Konflikte für Start/Stop testen.
- [ ] Active/List/Update/Delete testen.
- [ ] Overlap-Verhinderung via API testen.

### 8.4 Summary-Endpunkt testen
- [ ] UTC-Zeiträume testen.
- [ ] Inklusive from/to testen.
- [ ] ISO-Wochenstart testen.
- [ ] Nur abgeschlossene Einträge testen.

---

## Meilenstein 9 – CI/CD und Qualitätsgates

### 9.1 Pipeline-Basis
- [ ] CI-Workflow für Build + Unit Tests erstellen.
- [ ] Separaten Job für Integrationstests erstellen.
- [ ] Docker-Voraussetzungen in CI sicherstellen.

### 9.2 Branch-Bedingungen
- [ ] Integrationstests nur für Pull Requests ausführen.
- [ ] Integrationstests für main-Branch ausführen.
- [ ] Schneller Unit-Test-Lauf für alle Branches optional ergänzen.

### 9.3 Quality Gates
- [ ] Coverage-Gate in CI erzwingen.
- [ ] Testfehlschlag blockiert Merge.
- [ ] Artefakte (Testreports/Coverage) publizieren.

---

## Meilenstein 10 – Abschluss und Übergabe

### 10.1 Technische Dokumentation
- [ ] README für lokales Setup ergänzen.
- [ ] Umgebungsvariablen und Secrets dokumentieren.
- [ ] API-Endpunkte und Beispielrequests dokumentieren.
- [ ] Fehlermodell (ProblemDetails) dokumentieren.

### 10.2 Abnahme-Checkliste MVP
- [ ] Alle Muss-Anforderungen aus Fachspezifikation abgedeckt.
- [ ] Alle beschlossenen Architekturentscheidungen umgesetzt.
- [ ] Unit- und Integrationstests grün.
- [ ] Coverage-Gate erfüllt.
- [ ] Docker-Setup lauffähig und reproduzierbar.

---

