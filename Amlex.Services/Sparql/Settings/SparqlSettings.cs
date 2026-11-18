using System;

namespace Amlex.Services.Sparql.Settings;

public static class SparqlSettings
{
    public const string ApiEndpoint = "https://publications.europa.eu/webapi/rdf/sparql";
    public static string BuildQuery(int pageSize = 100, DateTime? since = null, DateTime? before = null)
    {
        string cursorFilter;

        if (before is not null)
        {
            string stamp = before.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            cursorFilter = $"""FILTER(?lastMod < "{stamp}"^^xsd:dateTime)""";
        }
        else if (since is not null)
        {
            string stamp = since.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            cursorFilter = $"""FILTER(?lastMod > "{stamp}"^^xsd:dateTime)""";
        }
        else
        {
            cursorFilter = ""; // full backfill – use carefully
        }

        return $$"""
            PREFIX cdm: <http://publications.europa.eu/ontology/cdm#>
            PREFIX cmr: <http://publications.europa.eu/ontology/cdm/cmr#>
            PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>

            SELECT DISTINCT
              ?work
              ?celex
              ?lastMod
              ?docDate
              ?inForce
              ?entryIntoForce
              ?deadline
              ?endOfValidity
              ?resType
              ?title
              ?language
              ?eurovoc
              ?amends
              ?repeals
              ?proposesToAmend

            WHERE {
              # --- EuroVoc concept set (food safety) ---
              VALUES ?concept {
                <http://eurovoc.europa.eu/6569>          # food safety
                <http://eurovoc.europa.eu/1279>          # food hygiene (deprecated but required)
                <http://eurovoc.europa.eu/1442>          # food inspection
                <http://eurovoc.europa.eu/1445>          # veterinary inspection
                <http://eurovoc.europa.eu/1590>          # foodstuffs legislation
                <http://eurovoc.europa.eu/2735>          # foodstuff
                <http://eurovoc.europa.eu/2531>          # food contamination
                <http://eurovoc.europa.eu/6052>          # food additive
                <http://eurovoc.europa.eu/c_60d786af>    # novel food
                <http://eurovoc.europa.eu/3469>          # pesticide residue
                <http://eurovoc.europa.eu/c_7951978e>    # drug residue
                <http://eurovoc.europa.eu/1755>          # animal disease
                <http://eurovoc.europa.eu/191>           # plant health control
                <http://eurovoc.europa.eu/1595>          # plant health legislation
                <http://eurovoc.europa.eu/6307>          # traceability
                <http://eurovoc.europa.eu/2078>          # food standard
                <http://eurovoc.europa.eu/1224>          # animal feedingstuffs
                <http://eurovoc.europa.eu/7126>          # EFSA
                <http://eurovoc.europa.eu/c_ee706406>    # food contact material
              }

              ?work cdm:work_is_about_concept_eurovoc ?concept .
              ?work cdm:resource_legal_id_celex ?celex .
              ?work cmr:lastModificationDate ?lastMod .

              # Only sector 3 (legislation) and 5 (preparatory)
              FILTER(STRSTARTS(STR(?celex), "3") || STRSTARTS(STR(?celex), "5"))

              {{cursorFilter}}

              # --- Optional metadata ---
              OPTIONAL { ?work cdm:work_date_document ?docDate }
              OPTIONAL { ?work cdm:resource_legal_in-force ?inForce }
              OPTIONAL { ?work cdm:resource_legal_date_entry-into-force ?entryIntoForce }
              OPTIONAL { ?work cdm:resource_legal_date_deadline ?deadline }
              OPTIONAL { ?work cdm:resource_legal_date_end-of-validity ?endOfValidity }
              OPTIONAL { ?work cdm:work_has_resource-type ?resType }

              # Titles (NL / EN / FR)
              OPTIONAL {
                ?expression cdm:expression_belongs_to_work ?work ;
                            cdm:expression_uses_language ?language ;
                            cdm:expression_title ?title .
                FILTER(?language IN (
                  <http://publications.europa.eu/resource/authority/language/NLD>,
                  <http://publications.europa.eu/resource/authority/language/ENG>,
                  <http://publications.europa.eu/resource/authority/language/FRA>
                ))
              }

              # EuroVoc concepts (can be multiple)
              OPTIONAL { ?work cdm:work_is_about_concept_eurovoc ?eurovoc }

              # Relations – useful for watchlist boost & relevance
              OPTIONAL {
                ?work cdm:resource_legal_amends_resource_legal ?amended .
                ?amended cdm:resource_legal_id_celex ?amends .
              }
              OPTIONAL {
                ?work cdm:resource_legal_repeals_resource_legal ?repealed .
                ?repealed cdm:resource_legal_id_celex ?repeals .
              }
              OPTIONAL {
                ?work cdm:resource_legal_proposes_to_amend_resource_legal ?proposed .
                ?proposed cdm:resource_legal_id_celex ?proposesToAmend .
              }
            }

            ORDER BY DESC(?lastMod) DESC(STR(?celex))
            LIMIT {{pageSize}}
            """;
    }
}