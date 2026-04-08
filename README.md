# JobAggregator

REST API agregujące oferty pracy z wielu źródeł. Aplikacja automatycznie pobiera ogłoszenia co godzinę i zapisuje je w bazie PostgreSQL, eliminując duplikaty.

## Funkcje

- Pobieranie ofert pracy z **Adzuna**
- Automatyczna synchronizacja w tle co 1 godzinę
- Upsert – aktualizacja istniejących ofert bez duplikatów
- Paginacja wyników
- Swagger UI do testowania API
- Architektura Clean Architecture (Domain / Application / Infrastructure / API)

## Architektura

```
JobAggregator.Domain         – encje (JobOffer), interfejsy (IJobRepository, IJobSource)
JobAggregator.Application    – opcje konfiguracyjne
JobAggregator.Infrastructure – implementacje: scrapery, repozytorium, EF Core, background job
JobAggregator.API            – kontrolery ASP.NET Core, konfiguracja DI
```

## Wymagania

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL (lokalnie lub przez Docker)
- Klucze API Adzuna (bezpłatne konto na [developer.adzuna.com](https://developer.adzuna.com))

## Uruchomienie

### 1. Baza danych

```bash
# Przykład z Dockerem
docker run --name jobdb -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=jobdb -p 5432:5432 -d postgres
```

### 2. Konfiguracja

Uzupełnij `appsettings.json` (lub użyj `dotnet user-secrets`):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=jobdb;Username=postgres;Password=postgres"
  },
  "Adzuna": {
    "AppId": "TWÓJ_APP_ID",
    "AppKey": "TWÓJ_APP_KEY",
    "Country": "pl"
  }
}
```

### 3. Migracje i uruchomienie

```bash
dotnet ef database update --project JobAggregator.Infrastructure --startup-project JobAggregator.API
dotnet run --project JobAggregator.API
```

Swagger dostępny pod: `http://localhost:5000/swagger`

## Endpointy API

| Metoda | Endpoint | Opis |
|--------|----------|------|
| `GET` | `/api/jobs` | Wszystkie oferty (posortowane po dacie pobrania) |
| `GET` | `/api/jobs/{id}` | Pojedyncza oferta po GUID |
| `GET` | `/api/jobs/paged?page=1&pageSize=20` | Oferty z paginacją (max 100 na stronę) |

### Przykładowa odpowiedź (paginacja)

```json
{
  "items": [...],
  "totalCount": 350,
  "page": 1,
  "pageSize": 20
}
```

## Model oferty

| Pole | Typ | Opis |
|------|-----|------|
| `id` | Guid | Wewnętrzny identyfikator |
| `title` | string | Stanowisko |
| `company` | string | Nazwa firmy |
| `location` | string | Lokalizacja (pełna) |
| `city` | string? | Miasto |
| `category` | string? | Kategoria zawodowa |
| `description` | string? | Treść ogłoszenia |
| `salaryMin` | decimal? | Wynagrodzenie od (PLN) |
| `salaryMax` | decimal? | Wynagrodzenie do (PLN) |
| `sourceName` | string | Źródło (`Adzuna` / `JustJoin.it`) |
| `sourceUrl` | string | Link do oryginalnego ogłoszenia |
| `fetchedAt` | DateTime | Czas pobrania (UTC) |
| `publishedAt` | DateTime? | Czas publikacji ogłoszenia |

## Źródła danych

| Źródło | API | Uwagi |
|--------|-----|-------|
| **Adzuna** | REST API (wymaga klucza) | 50 ofert na żądanie |

## Technologie

- ASP.NET Core 9
- Entity Framework Core + Npgsql (PostgreSQL)
- `IHostedService` – background job co 1h
- Swagger / OpenAPI
