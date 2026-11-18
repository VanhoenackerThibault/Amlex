using System.Text.Json.Serialization;

namespace Amlex.Models.Models.SparqlModels;

public class SparqlResponse
{
    [JsonPropertyName("head")]
    public SparqlHead Head { get; set; } = new();

    [JsonPropertyName("results")]
    public SparqlResults Results { get; set; } = new();
}