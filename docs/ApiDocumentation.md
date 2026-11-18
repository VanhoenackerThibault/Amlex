# AMLEX — CELLAR API documentation

This document describes the two external EU APIs used by AMLEX:

1. **CELLAR SPARQL endpoint** — metadata discovery and change detection.
2. **CELLAR REST document endpoint** — downloading the actual content of a legal act.

AMLEX uses SPARQL to discover and describe relevant EU legal acts. It uses the REST endpoint to download the XHTML or PDF representation of an act once AMLEX has identified it.

CELLAR is the Publications Office of the EU’s underlying content repository. EUR-Lex is the public website built on top of CELLAR. CELLAR stores both metadata and document content; its metadata is available as RDF and can be queried with SPARQL.

## API overview

| API | Base URL | Main purpose | AMLEX component |
|---|---|---|---|
| CELLAR SPARQL | `https://publications.europa.eu/webapi/rdf/sparql` | Search and retrieve metadata | `CellarSparqlClient` |
| CELLAR REST | `https://publications.europa.eu/resource/celex/{CELEX}` | Download a document representation | `CellarDocumentClient` |

| Concern | API to use |
|---|---|
| Find newly or recently modified relevant acts | SPARQL |
| Read CELEX, CELLAR ID, titles, dates, EuroVoc tags | SPARQL |
| Detect amendment, repeal, or proposed-amendment relationships | SPARQL |
| Download XHTML or PDF content | REST |
| Check whether content changed since the last download | REST |
| Store local review state, relevance, and change events | AMLEX PostgreSQL database |

## Authentication

No API key or OAuth flow is documented for the public CELLAR SPARQL and REST endpoints.

AMLEX should still:

- Send a descriptive `User-Agent` header.
- Identify the application and provide a contact address.
- Use conservative request rates and retries.
- Cache responses locally.
- Treat unexpected HTTP responses as operational events, not silent failures.

Example:

```http
User-Agent: AMLEX/1.0 (legal-monitoring; contact@example.eu)
```

## CELLAR SPARQL endpoint

### Endpoint

```text
POST https://publications.europa.eu/webapi/rdf/sparql
```

SPARQL is the query language for RDF. RDF represents information as triples:

```text
subject -- predicate --> object
```

Example:

```text
Legal act -- has EuroVoc topic --> food safety
Legal act -- has CELEX ID ------> 32002R0178
Legal act -- was modified on ---> 2026-11-17T10:00:00Z
```

### Request

Use HTTP `POST`, with the SPARQL query in a URL-encoded form body.

```http
POST https://publications.europa.eu/webapi/rdf/sparql
Content-Type: application/x-www-form-urlencoded
Accept: application/sparql-results+json
User-Agent: AMLEX/1.0 (legal-monitoring; contact@example.eu)

query=<URL-encoded SPARQL query>
```

Required headers:

| Header | Value | Purpose |
|---|---|---|
| `Content-Type` | `application/x-www-form-urlencoded` | Tells the endpoint that the query is in the request body |
| `Accept` | `application/sparql-results+json` | Requests SPARQL results as JSON |
| `User-Agent` | Descriptive application identifier | Identifies AMLEX and provides a contact point |

### Response

A successful request returns HTTP `200 OK` with a SPARQL JSON result document.

The important part of the response is:

```json
{
  "head": {
    "vars": ["work", "celex", "title", "eurovoc", "lastModified"]
  },
  "results": {
    "bindings": [
      {
        "work": {
          "type": "uri",
          "value": "http://publications.europa.eu/resource/cellar/..."
        },
        "celex": {
          "type": "literal",
          "value": "32002R0178"
        },
        "title": {
          "type": "literal",
          "value": "Regulation (EC) No 178/2002 ..."
        },
        "eurovoc": {
          "type": "uri",
          "value": "http://eurovoc.europa.eu/6569"
        },
        "lastModified": {
          "type": "literal",
          "value": "2026-11-17T10:00:00Z"
        }
      }
    ]
  }
}
```

Each object in `results.bindings` is one **binding**: one combination of the variables requested by the query.

A single legal act may produce multiple bindings because it can have:

- Multiple language titles.
- Multiple EuroVoc tags.
- Multiple resource types.
- Multiple relationships to other acts.

AMLEX must therefore use `DISTINCT` in SPARQL and upsert records locally rather than blindly inserting rows.

### CDM properties

CELLAR uses the **Common Data Model** (CDM). These are the property names AMLEX should use in SPARQL.

| Property | Meaning | AMLEX use |
|---|---|---|
| `cdm:resource_legal_id_celex` | CELEX identifier | Stable legal-act identifier |
| `cdm:work_is_about_concept_eurovoc` | EuroVoc subject concept | Topic discovery and classification |
| `cdm:work_date_document` | Legal document date | Metadata |
| `cmr:lastModificationDate` | Technical modification date | Incremental polling cursor |
| `cmr:creationDate` | Creation date | Metadata |
| `cdm:resource_legal_date_entry-into-force` | Entry into force | Metadata |
| `cdm:resource_legal_date_deadline` | Deadline | Metadata and reviewer alerts |
| `cdm:resource_legal_date_end-of-validity` | End of validity | Validity status |
| `cdm:work_has_resource-type` | Resource or document type | Document classification |
| `cdm:expression_title` | Title in a particular language | Localised titles |
| `cdm:expression_belongs_to_work` | Links an expression to its work | Language-specific title retrieval |
| `cdm:expression_uses_language` | Language of an expression | Title language matching |

Important relationship properties:

| Property | Meaning |
|---|---|
| `cdm:resource_legal_amends_resource_legal` | One act amends another |
| `cdm:resource_legal_repeals_resource_legal` | One act repeals another |
| `cdm:resource_legal_proposes_to_amend_resource_legal` | A proposal proposes to amend another act |
| `cdm:resource_legal_amendment_proposed_by_resource_legal` | Inverse of proposed amendment |

Do **not** use `work_id_celex`. It returned zero rows during verification. Use `cdm:resource_legal_id_celex`.

### EuroVoc filtering

EuroVoc is the EU’s multilingual thesaurus of subject concepts. A EuroVoc concept has a stable URI, for example:

```text
http://eurovoc.europa.eu/6569
```

AMLEX must always match on the full URI, not only the notation.

For example, `6569` is the notation for food safety, but notation numbers can collide across vocabularies. The full URI is the reliable identifier.

AMLEX uses the following configured concepts:

| AMLEX topic | Role | EuroVoc URI | Label |
|---|---|---|---|
| `FOOD_SAFETY` | Primary | `http://eurovoc.europa.eu/6569` | Food safety |
| `HYGIENE` | Primary | `http://eurovoc.europa.eu/1279` | Food hygiene |
| `INSPECTION` | Primary | `http://eurovoc.europa.eu/1442` | Food inspection |
| `INSPECTION` | Primary | `http://eurovoc.europa.eu/1445` | Veterinary inspection |
| `FOODSTUFFS_LAW` | Primary | `http://eurovoc.europa.eu/1590` | Foodstuffs legislation |
| `FOODSTUFF` | Broad | `http://eurovoc.europa.eu/2735` | Foodstuff |
| `CONTAMINATION` | Subcategory | `http://eurovoc.europa.eu/2531` | Food contamination |
| `ADD` | Subcategory | `http://eurovoc.europa.eu/6052` | Food additive |
| `NOVEL` | Subcategory | `http://eurovoc.europa.eu/c_60d786af` | Novel food |
| `MRL` | Subcategory | `http://eurovoc.europa.eu/3469` | Pesticide residue |
| `MRL` | Subcategory | `http://eurovoc.europa.eu/c_7951978e` | Drug residue |
| `DISEASES` | Subcategory | `http://eurovoc.europa.eu/1755` | Animal disease |
| `PLANT_HEALTH` | Subcategory | `http://eurovoc.europa.eu/191` | Plant health control |
| `PLANT_HEALTH` | Subcategory | `http://eurovoc.europa.eu/1595` | Plant health legislation |
| `TRACEABILITY` | Subcategory | `http://eurovoc.europa.eu/6307` | Traceability |
| `STANDARD` | Subcategory | `http://eurovoc.europa.eu/2078` | Food standard |
| `FEED` | Subcategory | `http://eurovoc.europa.eu/1224` | Animal feedingstuffs |
| `PACK` | Subcategory | `http://eurovoc.europa.eu/c_ee706406` | Food contact material |
| `EFSA` | Signal | `http://eurovoc.europa.eu/7126` | European Food Safety Authority |

The deprecated concept `http://eurovoc.europa.eu/1279` must remain included. Existing hygiene regulations `32004R0852` and `32004R0853` are still tagged with it and were not retagged to food safety (`6569`). Filtering only on `6569` would therefore miss core food-hygiene acts.

### Example: incremental discovery

This query finds relevant acts modified after a cursor timestamp.

```sparql
PREFIX cdm: <http://publications.europa.eu/ontology/cdm#>
PREFIX cmr: <http://publications.europa.eu/resource/cellar/common#>
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>

SELECT DISTINCT
  ?work
  ?celex
  ?title
  ?language
  ?eurovoc
  ?documentDate
  ?lastModified
WHERE {
  ?work cdm:resource_legal_id_celex ?celex ;
        cdm:work_is_about_concept_eurovoc ?eurovoc ;
        cdm:work_date_document ?documentDate ;
        cmr:lastModificationDate ?lastModified .

  OPTIONAL {
    ?expression cdm:expression_belongs_to_work ?work ;
                cdm:expression_uses_language ?language ;
                cdm:expression_title ?title .
  }

  VALUES ?eurovoc {
    <http://eurovoc.europa.eu/6569>
    <http://eurovoc.europa.eu/1279>
    <http://eurovoc.europa.eu/1442>
    <http://eurovoc.europa.eu/1445>
    <http://eurovoc.europa.eu/1590>
    <http://eurovoc.europa.eu/2735>
    <http://eurovoc.europa.eu/2531>
    <http://eurovoc.europa.eu/6052>
    <http://eurovoc.europa.eu/c_60d786af>
    <http://eurovoc.europa.eu/3469>
    <http://eurovoc.europa.eu/c_7951978e>
    <http://eurovoc.europa.eu/1755>
    <http://eurovoc.europa.eu/191>
    <http://eurovoc.europa.eu/1595>
    <http://eurovoc.europa.eu/6307>
    <http://eurovoc.europa.eu/2078>
    <http://eurovoc.europa.eu/1224>
    <http://eurovoc.europa.eu/7126>
    <http://eurovoc.europa.eu/c_ee706406>
  }

  FILTER(?lastModified >= "2026-11-16T00:00:00Z"^^xsd:dateTime)
}
ORDER BY ?lastModified ?celex
LIMIT 500
```

### Pagination and cursors

Do not use `OFFSET` for production backfills or incremental synchronisation.

Use a keyset cursor based on:

```text
(lastModificationDate, CELEX)
```

The cursor means: “Continue after this modification timestamp and CELEX.”

Rules:

- Store the cursor only after a successful database transaction commit.
- Apply an overlap window, for example one hour, to avoid missing records written during a run.
- When a run is truncated by a page limit, move the cursor to the oldest successfully processed timestamp, not the newest.
- Make ingestion idempotent: upsert by `CellarId` and/or `CELEX`.
- Treat duplicate SPARQL bindings as normal and merge them locally.

### Error handling

| HTTP status | Meaning | AMLEX action |
|---|---|---|
| `200 OK` | Query succeeded | Parse JSON bindings |
| `400 Bad Request` | Invalid SPARQL or malformed request | Log query and response; do not advance cursor |
| `401 Unauthorized` | Authentication required | Do not retry blindly; investigate configuration |
| `403 Forbidden` | Access denied | Do not retry blindly; investigate configuration |
| `429 Too Many Requests` | Rate limited | Back off with jitter and retry later |
| `500`, `502`, `503`, `504` | Temporary server issue | Retry with exponential backoff and jitter |

A failed SPARQL request must not advance the ingestion cursor.

## CELLAR REST document endpoint

### Endpoint

```text
GET https://publications.europa.eu/resource/celex/{CELEX}
```

Example:

```text
https://publications.europa.eu/resource/celex/32023R0915
```

This endpoint returns a concrete representation of a legal document. The same URL can return different formats depending on HTTP content negotiation.

### Content negotiation

AMLEX should request XHTML first, then PDF as a fallback.

| Header | Value | Purpose |
|---|---|---|
| `Accept` | `application/xhtml+xml` | Request machine-readable XHTML |
| `Accept` | `application/pdf` | Request PDF as fallback |
| `Accept-Language` | `nld`, `eng`, or `fra` | Request a specific language version |
| `User-Agent` | Descriptive application identifier | Identify AMLEX |

Do not rely on `text/html`; it may return `404`.

### Download strategy

For each legal act:

1. Try XHTML for Dutch (`nld`).
2. If XHTML returns `404`, try PDF for Dutch.
3. If both fail, repeat for English (`eng`).
4. If both fail, repeat for French (`fra`).
5. If all attempts fail, mark the document as unavailable and create an operational issue if appropriate.

A `404` for one format is an expected fallback condition, not necessarily an ingestion failure.

### Example: XHTML request

```http
GET https://publications.europa.eu/resource/celex/32002R0178
Accept: application/xhtml+xml
Accept-Language: nld
User-Agent: AMLEX/1.0 (legal-monitoring; contact@example.eu)
```

Expected successful response:

```http
200 OK
Content-Type: application/xhtml+xml
ETag: "some-etag-value"
Last-Modified: Wed, 18 Nov 2026 09:00:00 GMT
```

### Example: PDF fallback

```http
GET https://publications.europa.eu/resource/celex/32002R0178
Accept: application/pdf
Accept-Language: nld
User-Agent: AMLEX/1.0 (legal-monitoring; contact@example.eu)
```

Expected successful response:

```http
200 OK
Content-Type: application/pdf
ETag: "some-etag-value"
Last-Modified: Wed, 18 Nov 2026 09:00:00 GMT
```

### Conditional requests

AMLEX should avoid repeatedly downloading unchanged documents.

Use the stored `ETag` and `Last-Modified` values from the previous successful download.

#### ETag conditional request

```http
GET https://publications.europa.eu/resource/celex/32002R0178
Accept: application/xhtml+xml
Accept-Language: nld
If-None-Match: "previous-etag-value"
User-Agent: AMLEX/1.0
```

If the document has not changed, the server may return:

```http
304 Not Modified
```

AMLEX must not create a content-change event for HTTP `304`.

#### Last-Modified conditional request

```http
GET https://publications.europa.eu/resource/celex/32002R0178
Accept: application/xhtml+xml
Accept-Language: nld
If-Modified-Since: Wed, 18 Nov 2026 09:00:00 GMT
User-Agent: AMLEX/1.0
```

Again, HTTP `304 Not Modified` means the stored representation is still current.

### Response handling

| HTTP status | Meaning | AMLEX action |
|---|---|---|
| `200 OK` | New or changed representation returned | Store content, calculate SHA-256, save `ETag` and `Last-Modified` |
| `304 Not Modified` | Representation unchanged | Keep existing file and metadata; do not create a text-change event |
| `404 Not Found` | Requested representation/language unavailable | Try next format or language |
| `406 Not Acceptable` | Requested representation not available | Try next format or language |
| `429 Too Many Requests` | Rate limited | Back off and retry later |
| `500`, `502`, `503`, `504` | Temporary server issue | Retry with exponential backoff and jitter |

### Checksums and change detection

After a successful download, AMLEX calculates a SHA-256 checksum:

```csharp
var checksum = Convert.ToHexString(
    SHA256.HashData(bytes)).ToLowerInvariant();
```

Compare the new checksum with the stored checksum.

| Situation | AMLEX action |
|---|---|
| New checksum equals stored checksum | No content-change event |
| New checksum differs from stored checksum | Create `ContentChanged` event |
| HTTP `304` | No content-change event |
| Metadata changed but checksum unchanged | Create `MetadataChanged`, not `ContentChanged` |
| New relation discovered | Create `RelationChanged` |
| Related act changed | Create `RelatedActChanged` |

`cmr:lastModificationDate` indicates that CELLAR changed something about a resource. It does **not** prove that the legal text changed. AMLEX must use the SHA-256 checksum to determine whether the downloaded content itself changed.

## Watchlist synchronisation

The AMLEX watchlist contains priority legal acts that must always remain monitored.

Current watchlist:

| CELEX | Note |
|---|---|
| `32002R0178` | General Food Law |
| `32004R0852` | Food hygiene; tagged with EuroVoc `1279` only |
| `32004R0853` | Hygiene of animal products; tagged with EuroVoc `1279` only |
| `32017R0625` | Official controls; no `6569` tag |
| `32011R1169` | Food information to consumers |
| `32015R2283` | Novel food |
| `32023R0915` | Contaminants in food |

For watchlist acts, AMLEX should query directly by CELEX rather than relying only on EuroVoc-based discovery.

```sparql
PREFIX cdm: <http://publications.europa.eu/ontology/cdm#>
PREFIX cmr: <http://publications.europa.eu/resource/cellar/common#>

SELECT ?work ?celex ?lastModified
WHERE {
  ?work cdm:resource_legal_id_celex ?celex ;
        cmr:lastModificationDate ?lastModified .

  VALUES ?celex {
    "32002R0178"
    "32004R0852"
    "32004R0853"
    "32017R0625"
    "32011R1169"
    "32015R2283"
    "32023R0915"
  }
}
```

## AMLEX data flow

```text
1. Configuration
   AMLEX YAML defines:
   - EuroVoc concept URIs
   - internal topics
   - watchlist CELEX values

2. Metadata discovery
   CellarSparqlClient sends SPARQL to CELLAR.

3. Local storage
   AMLEX upserts:
   - LegalAct
   - LegalActTopic
   - LegalActRelation
   - IngestCursor

4. Content retrieval
   CellarDocumentClient downloads XHTML or PDF.

5. Change detection
   AMLEX compares:
   - metadata snapshots
   - SHA-256 checksums
   - relationships

6. Review workflow
   AMLEX creates ChangeEvents for human review.
   Reviewers assess relevance, assign teams, and publish an essence.
```

## Implementation cautions

- Do not use `OFFSET` pagination for large backfills.
- Do not advance a cursor before the corresponding database transaction commits.
- Do not assume one SPARQL binding equals one legal act.
- Do not treat a metadata modification as a text modification.
- Do not treat an XHTML `404` as a pipeline failure before trying PDF.
- Do not overwrite a known-good stored document after a failed download.
- Do not rely only on EuroVoc `6569` for food-safety discovery.
- Do not store EuroVoc notations as canonical identifiers; store full URIs.
- Do not automatically publish or make compliance decisions from change events; human reviewers remain responsible.
