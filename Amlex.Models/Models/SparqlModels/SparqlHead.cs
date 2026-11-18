using System.Text.Json.Serialization;

namespace Amlex.Models.Models.SparqlModels;

public class SparqlHead
{
    [JsonPropertyName("link")]
    public List<object> Link { get; set; } = [];

    [JsonPropertyName("vars")]
    public List<string> Vars { get; set; } = [];
}