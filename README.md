# AMLEX

AMLEX is an internal legal-monitoring application for EU food-safety legislation.

It discovers relevant EU legal acts through the European Union’s CELLAR repository, stores metadata locally, downloads document content when needed, detects changes, and creates review tasks for human experts.

AMLEX does **not** make legal-compliance decisions automatically. Human reviewers remain responsible for assessing relevance, assigning ownership, and publishing internal summaries.

## What AMLEX does

AMLEX monitors two broad categories of EU legal material:

- **Sector 3** — existing EU legislation.
- **Sector 5** — preparatory acts and legislative initiatives, such as European Commission proposals.

For each relevant legal act, AMLEX can:

- Discover the act through CELLAR SPARQL.
- Store stable identifiers and metadata locally.
- Download XHTML or PDF content through the CELLAR REST endpoint.
- Detect metadata, relationship, and content changes.
- Generate change events for human review.
- Assign topics and teams.
- Support an internal review workflow and short “essence” summaries.

## How it works

```text
CELLAR SPARQL
      |
      | metadata discovery
      v
AMLEX ingestion service
      |
      | store metadata, topics, relations, cursor
      v
Local database
      |
      | document download request
      v
CELLAR REST endpoint
      |
      | XHTML or PDF
      v
AMLEX local document store
      |
      | SHA-256 comparison
      v
Change events
      |
      v
Human reviewer workflow
```

## External data sources

### CELLAR SPARQL

CELLAR is the underlying EU Publications Office repository behind EUR-Lex. Its metadata is stored as RDF and can be queried using SPARQL.

AMLEX uses SPARQL to discover and describe legal acts:

- CELEX identifier
- CELLAR identifier
- Titles in Dutch, English, and French
- EuroVoc subject tags
- Document dates
- Modification dates
- Entry-into-force dates
- Deadlines
- Document types
- Amendment, repeal, and proposed-amendment relationships

Endpoint:

```text
https://publications.europa.eu/webapi/rdf/sparql
```

### CELLAR REST

Once AMLEX identifies a relevant legal act, it downloads the actual document content using the CELEX identifier.

Endpoint pattern:

```text
https://publications.europa.eu/resource/celex/{CELEX}
```

Example:

```text
https://publications.europa.eu/resource/celex/32023R0915
```

AMLEX requests XHTML first and falls back to PDF if XHTML is unavailable. It tries Dutch, English, and French in that order.

## Key concepts

| Term | Meaning |
|---|---|
| EUR-Lex | Public EU website for legal content |
| CELLAR | Underlying EU content repository and metadata platform behind EUR-Lex |
| CELEX | Stable identifier for an EU legal document, for example `32002R0178` |
| CELLAR ID | Stable UUID identifier for a CELLAR resource |
| SPARQL | Query language for RDF / linked data |
| RDF | Data model based on subject-predicate-object triples |
| CDM | CELLAR Common Data Model; the property names used by CELLAR |
| EuroVoc | EU multilingual thesaurus of subject concepts |
| Ingestion | Process that retrieves, normalises, and stores external data |
| Cursor | Stored high-water mark used for incremental synchronisation |
| ETag | HTTP version identifier used for conditional requests |
| SHA-256 checksum | Fingerprint of downloaded content used to detect actual content changes |
| Change event | Record that something changed and may require review |
| Watchlist | Small set of priority legal acts monitored closely |

## EuroVoc topics

AMLEX does not rely only on EuroVoc concept `6569` (food safety). It uses a configured set of full EuroVoc URIs, including deprecated concepts where needed.

The core topic set includes:

- Food safety
- Food hygiene
- Food inspection
- Veterinary inspection
- Foodstuffs legislation
- Food contamination
- Food additives
- Novel food
- Pesticide and drug residues
- Animal disease
- Plant health
- Traceability
- Food standards
- Animal feedingstuffs
- Food contact materials
- European Food Safety Authority (EFSA)

AMLEX always stores and matches full EuroVoc URIs, such as:

```text
http://eurovoc.europa.eu/6569
```

It does not rely on notation numbers alone, because notation numbers can collide across vocabularies.

## Watchlist

AMLEX maintains a watchlist of priority legal acts. These acts are monitored directly by CELEX, even if their EuroVoc tagging changes or does not match the configured discovery topics.

| CELEX | Description |
|---|---|
| `32002R0178` | General Food Law |
| `32004R0852` | Food hygiene |
| `32004R0853` | Hygiene of animal products |
| `32017R0625` | Official controls |
| `32011R1169` | Food information to consumers |
| `32015R2283` | Novel food |
| `32023R0915` | Contaminants in food |

## Technology stack

- C# / .NET
- ASP.NET Core Web API
- Entity Framework Core
- EF Core InMemory database for early development and simple local testing
- `HttpClient` for external APIs
- Background services or Hangfire for scheduled ingestion
- xUnit for testing
- React / Next.js or Blazor for the future frontend

## Database

AMLEX currently uses the **EF Core InMemory database provider**.

The InMemory provider keeps application data only in the running application process. When the application restarts, stored legal acts, topics, relations, downloaded-document metadata, change events, and ingestion cursors are lost.

This is suitable for early development, API prototyping, and simple unit tests. It is **not** suitable for production AMLEX data, durable ingestion cursors, reliable change-event history, or scheduled background jobs that must resume after a restart.

For persistent AMLEX environments, use PostgreSQL:

- Persistent legal acts and metadata
- Persistent ingestion cursors
- Persistent change events
- Reliable transactions
- Relational constraints and SQL queries
- Production-ready data storage

The application should keep its database provider configurable so it can use InMemory during early development and PostgreSQL when persistence is required.

## Project structure

A typical initial structure is:

```text
AMLEX/
├── .gitignore
├── README.md
├── AMLEX.sln
├── src/
│   ├── AMLEX.Api/
│   ├── AMLEX.Application/
│   ├── AMLEX.Domain/
│   └── AMLEX.Infrastructure/
└── tests/
    ├── AMLEX.UnitTests/
    └── AMLEX.IntegrationTests/
```

| Project | Purpose |
|---|---|
| `AMLEX.Api` | ASP.NET Core HTTP API and endpoints |
| `AMLEX.Application` | Ingestion, change detection, and workflow services |
| `AMLEX.Domain` | Entities, enums, and domain rules |
| `AMLEX.Infrastructure` | EF Core, database providers, CELLAR clients, file storage |
| `AMLEX.UnitTests` | Fast unit tests |
| `AMLEX.IntegrationTests` | Tests involving the database or external services |

## Local development

### Prerequisites

- .NET SDK
- Git
- Optional: Docker, for a future PostgreSQL setup

### Restore dependencies

```bash
dotnet restore
```

### Build

```bash
dotnet build
```

### Run tests

```bash
dotnet test
```

### Run the API

```bash
dotnet run --project src/AMLEX.Api
```

## Configuration

Non-secret configuration belongs in `appsettings.json`.

Example:

```json
{
  "Cellar": {
    "SparqlEndpoint": "https://publications.europa.eu/webapi/rdf/sparql",
    "DocumentBaseUrl": "https://publications.europa.eu/resource/celex/",
    "Languages": ["nld", "eng", "fra"]
  },
  "Database": {
    "Provider": "InMemory"
  }
}
```

Secrets must not be stored in source control.

For local development, use .NET user secrets:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Amlex" "Host=localhost;Database=amlex;Username=postgres;Password=your-local-password"
```

When PostgreSQL is introduced, use a configuration similar to:

```json
{
  "Database": {
    "Provider": "PostgreSQL"
  }
}
```

The actual connection string should remain in user secrets, environment variables, or a deployment secret store.

## Data model

Core entities include:

- `LegalAct`
- `LegalActTopic`
- `LegalActRelation`
- `RawDocument`
- `ChangeEvent`
- `IngestCursor`

The local database is intended to be the source of truth for:

- Acts already discovered by AMLEX
- Topic assignments
- Relationships between acts
- Downloaded document metadata
- Change events
- Review state

With the current InMemory database, this data exists only while the application is running.

## Change detection

AMLEX distinguishes between different types of change:

| Observation | Change event |
|---|---|
| New legal act discovered | `LegalActDiscovered` |
| Title, status, date, resource type, or EuroVoc tag changed | `MetadataChanged` |
| New amendment, repeal, or proposed-amendment relation | `RelationChanged` |
| Downloaded XHTML or PDF checksum changed | `ContentChanged` |
| XHTML unavailable but PDF becomes available | `RepresentationChanged` |
| Related legal act changed | `RelatedActChanged` |

A change to `cmr:lastModificationDate` does not automatically mean the legal text changed. AMLEX compares SHA-256 checksums to determine whether downloaded content actually changed.

## Ingestion rules

- Use `cmr:lastModificationDate` as the primary incremental cursor.
- Store the cursor only after a successful database commit.
- Use a keyset cursor of `(lastModificationDate, CELEX)`.
- Apply an overlap window, for example one hour.
- Make ingestion idempotent through upserts.
- Use `ETag` / `If-None-Match` and `Last-Modified` / `If-Modified-Since` conditional requests.
- Treat HTTP `304` as “not modified”; do not create a content-change event.
- Treat XHTML `404` as an expected fallback condition before trying PDF.

Because the current database is in memory, the ingestion cursor and previously discovered acts are reset when the application restarts. Persistent storage will be required before AMLEX can reliably resume incremental ingestion.

## Review workflow

AMLEX is designed to support human review, not replace it.

A reviewer can:

- See newly detected legal acts and changes.
- Assess relevance.
- Assign a topic and responsible team.
- Mark an event as reviewed, ignored, or escalated.
- Write and publish a short internal essence.

The system should never automatically publish legal analysis or make compliance decisions.

## Contributing

Before committing:

1. Ensure `.gitignore` is present.
2. Run `dotnet build`.
3. Run `dotnet test`.
4. Do not commit secrets, downloaded documents, build output, or IDE cache folders.
5. Keep changes small and focused.
6. Add or update tests for ingestion, parsing, change detection, and API behaviour.

## Useful commands

```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run all tests
dotnet test

# Run the API
dotnet run --project src/AMLEX.Api

# Initialise user secrets
dotnet user-secrets init

# Set a local secret
dotnet user-secrets set "ConnectionStrings:Amlex" "your-local-connection-string"
```

## Status

Implemented or partially implemented:

- Connector registry and configuration
- Incremental cursor fields
- Metadata backfill with resumable paging
- EuroVoc concept mapping and topic tagging
- Relevance scoring
- Team and topic assignment concepts
- Basic frontend and admin concepts
- InMemory database setup

Still to build:

- REST document download
- XHTML-to-PDF fallback
- Conditional requests
- ETag and Last-Modified storage
- Full change-event classification
- RDF relationship expansion
- Scheduled background ingestion
- PostgreSQL persistence and migrations
- Complete reviewer UI and workflow
