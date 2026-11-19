using System;

namespace Amlex.Services.Sparql.Settings;

public static class SparqlSettings
{
    public const string ApiEndpoint = "https://publications.europa.eu/webapi/rdf/sparql";

    private const string EuroVocValues = """
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
        <http://eurovoc.europa.eu/2746>
        <http://eurovoc.europa.eu/c_ee706406>
        """;


    public static string BuildDiscoveryQuery(
        int pageSize = 100,
        DateTime? since = null,
        DateTime? before = null)
    {
        string cursorFilter = BuildCursorFilter(since, before);

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
        WHERE {
          VALUES ?concept {
            {{EuroVocValues}}
          }

          ?work cdm:work_is_about_concept_eurovoc ?concept .
          ?work cdm:resource_legal_id_celex ?celex .
          ?work cmr:lastModificationDate ?lastMod .

          FILTER(STRSTARTS(STR(?celex), "3") || STRSTARTS(STR(?celex), "5"))

          {{cursorFilter}}

          OPTIONAL { ?work cdm:work_date_document ?docDate }
          OPTIONAL { ?work cdm:resource_legal_in-force ?inForce }
          OPTIONAL { ?work cdm:resource_legal_date_entry-into-force ?entryIntoForce }
          OPTIONAL { ?work cdm:resource_legal_date_deadline ?deadline }
          OPTIONAL { ?work cdm:resource_legal_date_end-of-validity ?endOfValidity }
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
        }
        ORDER BY DESC(?lastMod) DESC(STR(?celex))
        LIMIT {{pageSize}}
        """;
    }

    public static string BuildEuroVocQuery(IEnumerable<string> celexNumbers)
    {
        string values = BuildCelexValues(celexNumbers);

        return $$"""
        PREFIX cdm: <http://publications.europa.eu/ontology/cdm#>

        SELECT DISTINCT ?celex ?eurovoc
        WHERE {
          VALUES ?celex { {{values}} }

          ?work cdm:resource_legal_id_celex ?celex .
          ?work cdm:work_is_about_concept_eurovoc ?eurovoc .
        }
        """;
    }

    public static string BuildRelationsQuery(IEnumerable<string> celexNumbers)
    {
        string values = BuildCelexValues(celexNumbers);

        return $$"""
        PREFIX cdm: <http://publications.europa.eu/ontology/cdm#>

        SELECT ?celex ?rel ?otherCelex
        WHERE {
          VALUES ?celex { {{values}} }

          ?work cdm:resource_legal_id_celex ?celex .

          {
            ?work cdm:resource_legal_amends_resource_legal ?other .
            BIND("amends" AS ?rel)
          }
          UNION
          {
            ?work cdm:resource_legal_repeals_resource_legal ?other .
            BIND("repeals" AS ?rel)
          }
          UNION
          {
            ?work cdm:resource_legal_proposes_to_amend_resource_legal ?other .
            BIND("proposes_to_amend" AS ?rel)
          }

          ?other cdm:resource_legal_id_celex ?otherCelex .
        }
        """;
    }




    // -------------------------------------------------
    // Helpers
    // -------------------------------------------------
    private static string BuildCursorFilter(DateTime? since, DateTime? before)
    {
        if (before is not null)
        {
            string stamp = before.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            return $"""FILTER(?lastMod < "{stamp}"^^xsd:dateTime)""";
        }

        if (since is not null)
        {
            string stamp = since.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            return $"""FILTER(?lastMod > "{stamp}"^^xsd:dateTime)""";
        }

        return "";
    }

    private static string BuildCelexValues(IEnumerable<string> celexNumbers)
    {
        return string.Join(
            " ",
            celexNumbers
                .Where(celex => !string.IsNullOrWhiteSpace(celex))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(celex => $"\"{EscapeLiteral(celex)}\"^^xsd:string"));
    }

    private static string EscapeLiteral(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }
}