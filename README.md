
```markdown
# AMLEX

Internal legal-monitoring application for **EU food-safety legislation**.

AMLEX discovers relevant EU legal acts through the Publications Office **CELLAR** repository (SPARQL), stores metadata locally, and keeps the dataset up to date on an hourly basis. It helps teams see what changed — it does **not** make compliance decisions.

---

## What it does today

- Discovers EU legal acts related to food safety via CELLAR SPARQL
- Filters on a curated EuroVoc concept set (including hygiene, inspection, contaminants, novel food, EFSA, …)
- Covers **sector 3** (binding legislation) and **sector 5** (preparatory acts)
- Stores core metadata: CELEX, CELLAR id, titles (NL/EN/FR), dates, in-force status, resource type
- Enriches acts with:
  - EuroVoc topics (`LegalActTopic`)
  - Relations such as amends / repeals / proposes to amend (`LegalActRelation`)
- Upserts by CELEX and only updates when `lastModificationDate` is newer
- Refreshes data about once per hour (middleware + fetch history)
- Shows a simple dashboard of imported acts

## What it does **not** do yet

- Download full XHTML/PDF content
- Relevance scoring / triage workflow
- Team assignment
- Full reviewer UI / analysis publishing
- Background job queue (ingestion currently runs in-request)

---

## How it works

```text
Request comes in
      |
      v
RefreshSparqlDataMiddleware
  (if last fetch > 1 hour ago)
      |
      v
CELLAR SPARQL
  1. Discovery query  → LegalAct (basic fields + titles)
  2. EuroVoc query    → LegalActTopic
  3. Relations query  → LegalActRelation
      |
      v
SQL Server (upsert by CELEX)
      |
      v
Home dashboard
```

### Data sources

| Source | Endpoint | Used for |
|--------|----------|----------|
| CELLAR SPARQL | `https://publications.europa.eu/webapi/rdf/sparql` | Discovery, titles, EuroVoc, relations |

CELEX values are stored as plain identifiers (e.g. `32002R0178`), not full URLs.

---

## Solution structure

```text
Amlex/
├── Amlex.Interfaces/     # Service contracts
├── Amlex.Models/         # Entities, SPARQL DTOs, mappers, enums
├── Amlex.Services/       # SPARQL client, ingestion, DB services
├── Amlex.Repository/     # EF Core DbContext + migrations
├── Amlex.Mvc/            # ASP.NET Core MVC app + UI
└── docs/                 # CELLAR / API notes
```

### Main entities

| Entity | Role |
|--------|------|
| `LegalAct` | Core act metadata |
| `LegalActTopic` | EuroVoc concepts linked to an act |
| `LegalActRelation` | amends / repeals / proposes_to_amend |
| `LegalActsFetchHistory` | Last successful fetch timestamp |

---

## Tech stack

- .NET / C#
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server (configurable; InMemory possible for quick tests)
- HttpClient → CELLAR SPARQL
- Bootstrap + custom AM Norman–inspired UI

---

## Getting started

### Prerequisites

- .NET SDK
- SQL Server (LocalDB / full instance)
- Network access to `publications.europa.eu`

### Configure

Set the connection string (user secrets or `appsettings.Development.json`):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=Amlex;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Run migrations

```bash
dotnet ef database update --project Amlex.Repository --startup-project Amlex.Mvc
```

### Run the app

```bash
dotnet run --project Amlex.Mvc
```

Open the site (see `launchSettings.json`, typically `https://localhost:7242`).

On first requests, the middleware may trigger a SPARQL refresh if the last fetch is older than one hour (or missing).

### Manual backfill / test endpoints

Use the testing API controller (development) to trigger discovery / enrichment if needed.

---

## Ingestion design

1. **Discovery** – lightweight SPARQL query (identity, dates, titles, resource type)
2. **Upsert** – insert new CELEX or update when `LastModifiedAt` is newer
3. **Enrichment** (separate queries, chunked by CELEX list):
   - EuroVoc → `LegalActTopic`
   - Relations → `LegalActRelation`
4. **Fetch history** – store last successful run so refreshes stay ~hourly

EuroVoc filtering uses full URIs (e.g. `http://eurovoc.europa.eu/6569`).  
Deprecated concept `1279` (food hygiene) is kept on purpose so hygiene regulations are not missed.

---

## Configuration notes

- Named HttpClient: `SPARQL`
- Middleware: `RefreshSparqlDataMiddleware` (registered in `Program.cs`)
- SPARQL queries live in `Amlex.Services/Sparql/Settings/SparqlSettings.cs`

Optional logging (reduce HttpClient noise):

```json
{
  "Logging": {
    "LogLevel": {
      "System.Net.Http.HttpClient": "Warning"
    }
  }
}
```

---

## Status

**Implemented**

- CELLAR SPARQL discovery
- Title aggregation (NL/EN/FR)
- EuroVoc + relation enrichment
- CELEX upsert with newer-lastMod rule
- Fetch history + hourly refresh middleware
- Basic dashboard UI

**Planned**

- Relevance scoring
- Competence teams / assignment
- Change events & review workflow
- Document download (XHTML/PDF) + checksums
- Stronger cursor / keyset pagination for large backfills
- Production background jobs instead of in-request refresh

---

## Docs

- [`docs/ApiDocumentation.md`](docs/ApiDocumentation.md) — CELLAR SPARQL / REST notes

---

## License / usage

Internal project. Not intended as an automated compliance tool; human review remains required.
```