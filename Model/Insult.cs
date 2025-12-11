using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Insulter.Model;
public class Insult {
    [JsonPropertyName("number")]
    [Key]
    public int Number { get; set; }
    [JsonPropertyName("language")]
    public string? Language { get; set; }
    [JsonPropertyName("created")]
    public string? Created { get; set; }
    [JsonPropertyName("shown")]
    public string? Shown { get; set; }
    [JsonPropertyName("insult")]
    public string? Text { get; set; }
}

