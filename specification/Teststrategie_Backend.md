
# Teststrategie – Backend der Zeiterfassungsanwendung
**Version 1.0**

---

## 1. Zielsetzung

Diese Teststrategie beschreibt, wie die Qualität und Funktionalität des Backends systematisch durch automatisierte Tests sichergestellt wird. Im Fokus stehen **Unit Tests zur Absicherung der fachlichen Logik** und **Integrationstests zur Prüfung der API-Endpunkte inklusive Datenbankzugriff**.

---

## 2. Testarten & Abdeckung

| Testart           | Ziel                                                         | Abdeckung                                   |
|-------------------|--------------------------------------------------------------|---------------------------------------------|
| Unit Tests        | Isolierte Prüfung der Geschäftslogik                        | `Domain`, `Application` (100 % angestrebt)  |
| Integrationstests | End-to-End-Tests auf API-Ebene mit echter Datenbank         | Jeder Controller                            |

---

## 3. Unit Tests

### Zielsetzung
- Sicherstellen der Korrektheit und Stabilität von Fachlogik
- Tests laufen unabhängig und isoliert

### Testprojekte
- Für jedes Projekt, zu dem Unit Tests gehören, wird ein Projekt mit dem Suffix `.UnitTests` erstellt
- Die Testprojekte befinden sich **im selben Ordner** wie die getesteten Projekte (hier: `backend`)

**Beispiele:**
| Codeprojekt                  | Testprojekt                          |
|-----------------------------|--------------------------------------|
| Zeiterfassung.Domain        | Zeiterfassung.Domain.UnitTests       |
| Zeiterfassung.Application   | Zeiterfassung.Application.UnitTests  |

### Technologien
- Testframework: `xUnit`
- Mocking: `NSubstitute` (nur wenn nötig)
- Assertions: Standard `Assert` aus `xUnit`

### Abdeckung
- `Zeiterfassung.Domain`: Entitäten, Value Objects, Regeln
- `Zeiterfassung.Application`: Services, Validierungen, Use Cases
- Keine externen Abhängigkeiten (keine DB, kein Netzwerk)

### Namenskonventionen
- Klassen: `XyzTests.cs`
- Methoden: `MethodName_Conditions_ExpectedBehaviour`

**Beispiel:**
```csharp
CalculateDuration_EndTimeAfterStartTime_ReturnsCorrectValue()
```

---

## 4. Integrationstests

### Zielsetzung
- Prüfung des Zusammenspiels realer Komponenten über HTTP und PostgreSQL
- Absicherung der API-Endpunkte

### Testprojekte
- Für jedes getestete Modul wird ein Projekt mit dem Suffix `.IntegrationTests` erstellt
- Die Testprojekte liegen ebenfalls im Ordner `backend`

**Beispiel:**
| Codeprojekt               | Testprojekt                           |
|---------------------------|----------------------------------------|
| Zeiterfassung.API         | Zeiterfassung.API.IntegrationTests     |

### Infrastruktur
- Verwendung von **Testcontainers for .NET** zur Bereitstellung einer echten PostgreSQL-Datenbank für jeden Testlauf
- Automatische Anwendung von EF Core-Migrationen beim Start
- Isolierte Testdaten je Testfall

### Abdeckung
- Für **jeden Controller** wird ein Satz an API-Tests geschrieben
- Getestet werden:
  - Erfolgreiche und fehlerhafte Requests
  - Authentifizierung & Autorisierung
  - Validierungen
  - Datenpersistenz

### Namenskonventionen
- Klassen: `XyzControllerTests.cs`
- Methoden: `MethodName_Conditions_ExpectedBehaviour`

**Beispiel:**
```csharp
PostTimeEntry_ValidInput_CreatesEntryInDatabase()
```

### CI-Integration
- Eigener CI-Job für Integrationstests
- Voraussetzung: Docker-Laufzeit verfügbar im Build

---

## 5. Organisation & Ausführung

### Projektstruktur (Beispiel für backend-Verzeichnis)
```plaintext
backend/
├── Zeiterfassung.API/
├── Zeiterfassung.API.IntegrationTests/
├── Zeiterfassung.Application/
├── Zeiterfassung.Application.UnitTests/
├── Zeiterfassung.Domain/
├── Zeiterfassung.Domain.UnitTests/
├── Zeiterfassung.Infrastructure/
```

### Ausführung
- Lokal über `dotnet test` (direkt oder über IDE)
- Automatisiert über CI/CD-Pipeline bei jedem Commit oder Pull Request
- Unit- und Integrationstests werden separat ausgeführt

---

## 6. Ausblick (optional)

- Code Coverage Analyse mit `coverlet`
- Erweiterung um End-to-End-Tests (z. B. mit Flutter-UI)
- Performanztests bei großer Datenmenge
- Mutation Testing (z. B. mit Stryker.NET)
