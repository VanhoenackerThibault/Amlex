using System;

namespace Amlex.Services.Sparql.Settings;

public static class SparqlSettings
{
    public const string ApiEndpoint = "https://publications.europa.eu/webapi/rdf/sparql";


    public static string BuildQuery()
    {

        return $$""""
        PREFIX cdm: <http://publications.europa.eu/ontology/cdm#>
        PREFIX cmr: <http://publications.europa.eu/ontology/cdm/cmr#>
        PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>

        SELECT DISTINCT
          ?work
          ?celex
          ?lastMod
          ?docDate
          ?inForce
          ?title
          ?language
          ?eurovoc
          ?resType

        WHERE {
          VALUES ?concept {
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

          ?work cdm:work_is_about_concept_eurovoc ?concept .
          ?work cdm:resource_legal_id_celex ?celex .
          ?work cmr:lastModificationDate ?lastMod .

          OPTIONAL {
            ?work cdm:work_date_document ?docDate .
          }

          OPTIONAL {
            ?work cdm:resource_legal_in-force ?inForce .
          }

          OPTIONAL {
            ?work cdm:work_has_resource-type ?resType .
          }

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

          FILTER(
            STRSTARTS(STR(?celex), "3") ||
            STRSTARTS(STR(?celex), "5")
          )

          FILTER(?lastMod > "2026-11-17T00:00:00.0000000Z"^^xsd:dateTime)
        }

        ORDER BY DESC(?lastMod) DESC(STR(?celex))
        LIMIT 500
        """";
    }

    public static string BackFillQuery(int pageSize = 500, DateTime? lastModifiedCursor = null)
    {
        string cursorFilter = lastModifiedCursor is null? string.Empty : $"""
                FILTER(?lastMod < "{lastModifiedCursor:yyyy-MM-ddTHH:mm:ss.fffzzz}"^^xsd:dateTime)
            """;

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
          ?title
          ?language
          ?eurovoc
          ?resType

        WHERE {
          VALUES ?concept {
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

          ?work cdm:work_is_about_concept_eurovoc ?concept .
          ?work cdm:resource_legal_id_celex ?celex .
          ?work cmr:lastModificationDate ?lastMod .

          OPTIONAL { ?work cdm:work_date_document ?docDate }
          OPTIONAL { ?work cdm:resource_legal_in-force ?inForce }
          OPTIONAL { ?work cdm:work_has_resource-type ?resType }

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

          FILTER(
            STRSTARTS(STR(?celex), "3") ||
            STRSTARTS(STR(?celex), "5")
          )

          {{cursorFilter}}
        }

        ORDER BY DESC(?lastMod) DESC(STR(?celex))
        LIMIT {{pageSize}}
        OFFSET 0
        """;
    }
}