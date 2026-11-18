using System.Text.Json.Serialization;

namespace Amlex.Models.Models.SparqlModels;

public class SparqlResults
{
    [JsonPropertyName("distinct")]
    public bool? Distinct { get; set; }

    [JsonPropertyName("ordered")]
    public bool? Ordered { get; set; }

    [JsonPropertyName("bindings")]
    public List<SparqlBinding> Bindings { get; set; } = [];
}