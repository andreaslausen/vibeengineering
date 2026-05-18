# Environment-Variablen für docker-compose

## API-Service
- **ASPNETCORE_ENVIRONMENT**: Entwicklungsumgebung (z.B. Development)
- **ConnectionStrings__DefaultConnection**: Verbindungszeichenfolge zur PostgreSQL-Datenbank (z.B. Host=db;Database=zeiterfassung;Username=postgres;Password=postgres)
- **Jwt__SecretKey**: Geheimer Schlüssel für JWT-Signatur (mind. 32 Zeichen für HS256)
- **Jwt__Issuer**: JWT-Issuer (z.B. zeiterfassung-api)
- **Jwt__Audience**: JWT-Audience (z.B. zeiterfassung-clients)
- **Jwt__AccessTokenExpirationMinutes**: Access Token Gültigkeit in Minuten (z.B. 15)
- **Jwt__RefreshTokenExpirationDays**: Refresh Token Gültigkeit in Tagen (z.B. 7)

## DB-Service
- **POSTGRES_DB**: Name der Datenbank (z.B. zeiterfassung)
- **POSTGRES_USER**: Benutzername (z.B. postgres)
- **POSTGRES_PASSWORD**: Passwort (z.B. postgres)

Alle Variablen sind in der `docker-compose.yml` dokumentiert und können dort angepasst werden.