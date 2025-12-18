using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Insulter.Model;
public class Insult {
    [Key]
    public int Id { get; set; }

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("created")]
    public string? Created { get; set; }

    [JsonPropertyName("insult")]
    public string? Text { get; set; }
}

