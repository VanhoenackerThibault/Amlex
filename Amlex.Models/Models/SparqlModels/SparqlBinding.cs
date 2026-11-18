using System.Text.Json.Serialization;

namespace Amlex.Models.Models.SparqlModels;

public class SparqlBinding
{
    [JsonPropertyName("work")]
    public SparqlValue? Work { get; set; }

    [JsonPropertyName("celex")]
    public SparqlValue? Celex { get; set; }

    [JsonPropertyName("lastMod")]
    public SparqlValue? LastModified { get; set; }

    [JsonPropertyName("docDate")]
    public SparqlValue? DocumentDate { get; set; }

    [JsonPropertyName("inForce")]
    public SparqlValue? InForce { get; set; }

    [JsonPropertyName("entryIntoForce")]
    public SparqlValue? EntryIntoForce { get; set; }

    [JsonPropertyName("deadline")]
    public SparqlValue? Deadline { get; set; }

    [JsonPropertyName("endOfValidity")]
    public SparqlValue? EndOfValidity { get; set; }

    [JsonPropertyName("resType")]
    public SparqlValue? ResourceType { get; set; }

    [JsonPropertyName("title")]
    public SparqlValue? Title { get; set; }

    [JsonPropertyName("language")]
    public SparqlValue? Language { get; set; }

    [JsonPropertyName("eurovoc")]
    public SparqlValue? EuroVoc { get; set; }

    [JsonPropertyName("amends")]
    public SparqlValue? Amends { get; set; }

    [JsonPropertyName("repeals")]
    public SparqlValue? Repeals { get; set; }

    [JsonPropertyName("proposesToAmend")]
    public SparqlValue? ProposesToAmend { get; set; }
}