using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Insulter.Model;

namespace Insulter.Services;

public class InsultApiResponse {
    public string? insult { get; set; }
    public string? created { get; set; }
    public string? shown { get; set; }
    public string? language { get; set; }
    public int? number { get; set; }
}

public static class InsultService {
    private static readonly HttpClient _http = new HttpClient();

    public static async Task<string> GetRawJsonAsync(string lang = "en") {
        var url = $"https://evilinsult.com/generate_insult.php?lang={lang}&type=json";
        using var resp = await _http.GetAsync(url);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadAsStringAsync();
    }

    public static async Task<string?> GetInsultAsync(string lang = "en") {
        var json = await GetRawJsonAsync(lang);
        var data = JsonSerializer.Deserialize<InsultApiResponse>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });
        return data?.insult;
    }

    public static async Task<Insult?> GetInsultObjectAsync(string lang = "en") {
        var json = await GetRawJsonAsync(lang);
        var insult = JsonSerializer.Deserialize<Insult>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });
        return insult;
    }
}
