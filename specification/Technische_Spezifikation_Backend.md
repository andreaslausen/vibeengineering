
# Technische Spezifikation – Backend der Zeiterfassungsanwendung
**Version 1.1**

---

## 1. Zielsetzung

Das Backend bildet die technische Grundlage für eine persönliche Zeiterfassungsanwendung. Es stellt eine moderne, REST-basierte API bereit, über die sich Benutzer registrieren, authentifizieren und ihre Arbeitszeiten per Start-/Stopp-Mechanismus erfassen können. Darüber hinaus können Zeitblöcke eingesehen, bearbeitet, gelöscht und als Tages- oder Zeitraumsummen ausgewertet werden. Die Architektur ist modular aufgebaut, skalierbar und für eine spätere Erweiterung vorbereitet.

---

## 2. Technologie-Stack

| Komponente         | Entscheidung                                  |
|--------------------|-----------------------------------------------|
| Programmiersprache | C#                                             |
| Framework          | ASP.NET Core (aktuelle Version)               |
| Authentifizierung  | JWT mit Refresh Tokens                        |
| Datenbank          | PostgreSQL                                     |
| ORM                | Entity Framework Core                          |
| Hosting            | Docker + docker-compose                        |
| API-Stil           | REST (JSON-basiert)                            |

- Es wird stets die **aktuellste .NET-Version** verwendet, unabhängig vom LTS-Status.
- Die Anwendung wird containerisiert entwickelt und bereitgestellt.

---

## 3. Authentifizierung & Sicherheit

Die Authentifizierung erfolgt über **JWT**. Beim Login wird ein **Access Token** und ein **Refresh Token** generiert:

- **Access Token**: Lebensdauer ca. 15–30 Minuten
- **Refresh Token**: Lebensdauer ca. 7–30 Tage, wird in der DB gespeichert
- Refresh Token kann für neue Access Tokens verwendet werden
- Token werden bei Logout invalidiert

**Sicherheitsmaßnahmen:**
- Passwörter werden sicher gehasht (z. B. bcrypt)
- Zugriff nur über HTTPS
- Zugriff auf eigene Daten durch Zugriffskontrolle

---

## 4. Datenmodell (relational, mit EF Core)

### Tabelle: `users`
- id: UUID (PK)
- username: TEXT, UNIQUE, NOT NULL
- email: TEXT (optional)
- password_hash: TEXT, NOT NULL
- created_at: TIMESTAMP WITH TIME ZONE

### Tabelle: `time_entries`
- id: UUID (PK)
- user_id: UUID (FK)
- start_time: TIMESTAMP WITH TIME ZONE
- end_time: TIMESTAMP WITH TIME ZONE (nullable)
- duration: INTERVAL (optional)
- note: TEXT (optional)
- category: TEXT (optional)
- created_at: TIMESTAMP WITH TIME ZONE
- updated_at: TIMESTAMP WITH TIME ZONE

### Tabelle: `refresh_tokens`
- id: UUID (PK)
- user_id: UUID (FK)
- token: TEXT, UNIQUE
- expires_at: TIMESTAMP WITH TIME ZONE
- created_at: TIMESTAMP WITH TIME ZONE
- revoked_at: TIMESTAMP WITH TIME ZONE (nullable)

---

## 5. API-Endpunkte

### /auth
- `POST /auth/register`
- `POST /auth/login`
- `POST /auth/refresh`
- `POST /auth/logout`
- `GET /auth/me`

### /time
- `POST /time/start`
- `POST /time/stop`
- `GET /time/active`
- `GET /time/list?from=…&to=…`
- `PUT /time/{id}`
- `DELETE /time/{id}`

### /time/summary
- `GET /time/summary?from=…&to=…`

---

## 6. Architektur & Projektstruktur

### Struktur:

```
project-root/
├── backend/
│   ├── Zeiterfassung.API/
│   ├── Zeiterfassung.Application/
│   ├── Zeiterfassung.Domain/
│   ├── Zeiterfassung.Infrastructure/
│   └── Dockerfile
├── docker-compose.yml
```

### Verantwortlichkeiten:

| Projekt                      | Aufgabe                                                |
|-----------------------------|---------------------------------------------------------|
| Zeiterfassung.API            | REST-API, Routing, Middleware                          |
| Zeiterfassung.Application   | Business-Logik, DTOs, Services                         |
| Zeiterfassung.Domain        | Entitäten, Value Objects, Logik (ohne Infrastruktur)   |
| Zeiterfassung.Infrastructure| EF Core, Repositories, Token-Verwaltung, Logging       |

---

## 7. Infrastruktur & Hosting

- Entwicklung und Deployment per Docker
- Lokales Setup über `docker-compose`
- PostgreSQL-Container mit Volumes
- Späteres Deployment auf Azure, AWS, Hetzner etc. möglich

---

## 8. Zeitzonen & Sprache

- Alle Zeitangaben in **UTC**
- Lokale Umrechnung im Frontend oder über API
- API-Sprache: **Englisch**
- Lokalisierung aktuell nicht vorgesehen

---

## 9. Fehlerbehandlung

### Fehlerstruktur (Beispiel):

```json
{
  "error": "ValidationError",
  "message": "Start time must not be in the future."
}
```

### Fehlerarten:
- ValidationError
- AuthenticationFailed
- Unauthorized
- NotFound
- InternalServerError

### Statuscodes:
- 400, 401, 403, 404, 500

Fehler werden über zentrale Middleware behandelt.

---

## 10. Logging

- Verwendung von Serilog (alternativ Microsoft Logging)
- Log-Level: Information, Warning, Error
- Logging von:
  - Authentifizierungsvorgängen
  - Start/Stop-Aktionen
  - System- und API-Fehlern

---

## 11. Monitoring & Health Checks

- `GET /health`: Gibt 200 OK bei funktionierender API + DB
- Optional: Anbindung an Prometheus, Grafana, Application Insights

---

## 12. Erweiterungen (optional)

- E-Mail-Funktionalität
- CSV-/PDF-Export
- WebSockets (Live-Erfassung)
- Hintergrundjobs
- Rollen-/Rechtemanagement
