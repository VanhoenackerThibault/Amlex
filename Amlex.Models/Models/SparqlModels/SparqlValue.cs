using System.Text.Json.Serialization;

namespace Amlex.Models.Models.SparqlModels;

public class SparqlValue
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("datatype")]
    public string? DataType { get; set; }
}