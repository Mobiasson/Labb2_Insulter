using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Insulter.Model;

public class InsultApiResponse {
    public string? insult { get; set; }
    public string? created { get; set; }
    public string? shown { get; set; }
}

public static class InsultService {
    private static readonly HttpClient _http = new HttpClient();
    public static async Task<string?> GetInsultAsync(string lang = "en") {
        var url = $"https://evilinsult.com/generate_insult.php?lang={lang}&type=json";
        using var resp = await _http.GetAsync(url);
        resp.EnsureSuccessStatusCode();
        await using var stream = await resp.Content.ReadAsStreamAsync();
        var data = await JsonSerializer.DeserializeAsync<InsultApiResponse>(stream, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });
        return data?.insult;
    }
}
