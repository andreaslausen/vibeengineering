# Environment-Variablen für docker-compose

## API-Service
- **ASPNETCORE_ENVIRONMENT**: Entwicklungsumgebung (z.B. Development)
- **ConnectionStrings__DefaultConnection**: Verbindungszeichenfolge zur PostgreSQL-Datenbank (z.B. Host=db;Database=zeiterfassung;Username=postgres;Password=postgres)
- **Jwt__Key**: Geheimer Schlüssel für JWT-Signatur (z.B. your_jwt_secret_key)
- **Jwt__Issuer**: JWT-Issuer (z.B. zeiterfassung)
- **Jwt__Audience**: JWT-Audience (z.B. zeiterfassung_users)

## DB-Service
- **POSTGRES_DB**: Name der Datenbank (z.B. zeiterfassung)
- **POSTGRES_USER**: Benutzername (z.B. postgres)
- **POSTGRES_PASSWORD**: Passwort (z.B. postgres)

Alle Variablen sind in der `docker-compose.yml` dokumentiert und können dort angepasst werden.