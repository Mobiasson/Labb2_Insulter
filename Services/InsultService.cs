using System.Net.Http;
using System.Text.Json;
using Insulter.Model;
using Microsoft.EntityFrameworkCore;

namespace Insulter.Services;

public class InsultApiResponse {
    public string? insult { get; set; }
    public string? created { get; set; }
    public string? shown { get; set; }
    public string? language { get; set; }
    public object? number { get; set; }
}

public static class InsultService {
    private static readonly HttpClient _http = new HttpClient();
    private static readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    public static async Task<string> GetRawJsonAsync(string lang = "en") {
        var url = $"https://evilinsult.com/generate_insult.php?lang={lang}&type=json";
        using var resp = await _http.GetAsync(url);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadAsStringAsync();
    }

    public static async Task<string?> GetInsultAsync(string lang = "en") {
        var json = await GetRawJsonAsync(lang);
        var data = JsonSerializer.Deserialize<InsultApiResponse>(json, _jsonOpts);
        return data?.insult;
    }

    public static async Task<Insult?> GetInsultObjectAsync(string lang = "en") {
        var json = await GetRawJsonAsync(lang);
        var dto = JsonSerializer.Deserialize<InsultApiResponse>(json, _jsonOpts);
        if(dto == null || string.IsNullOrWhiteSpace(dto.insult)) return null;

        int? apiNumber = null;
        if(dto.number is JsonElement je) {
            if(je.ValueKind == JsonValueKind.Number && je.TryGetInt32(out var n)) apiNumber = n;
            else if(je.ValueKind == JsonValueKind.String && int.TryParse(je.GetString(), out var ns)) apiNumber = ns;
        } else if(dto.number is string s && int.TryParse(s, out var n1)) apiNumber = n1;
        else if(dto.number is int n2) apiNumber = n2;
        return new Insult {
            Text = dto.insult,
            Created = dto.created,
            Language = dto.language,
        };
    }

    public static async Task<int> FetchAndSaveBatchAsync(int count, string lang = "en", CancellationToken ct = default) {
        var collected = new List<Insult>(count);
        using var db = new InsultContext();
        await db.Database.MigrateAsync(ct);
        for(int i = 0; i < count; i++) {
            ct.ThrowIfCancellationRequested();
            var insult = await GetInsultObjectAsync(lang);
            if(insult == null || string.IsNullOrWhiteSpace(insult.Text)) {
                await Task.Delay(300, ct);
                continue;
            }
            bool exists = await db.Insults.AnyAsync(x => x.Text == insult.Text, ct);
            if(exists || collected.Any(x => x.Text == insult.Text)) {
                await Task.Delay(200, ct);
                continue;
            }
            collected.Add(insult);
            await Task.Delay(300, ct);
        }
        if(collected.Count == 0) return 0;
        db.Insults.AddRange(collected);
        return await db.SaveChangesAsync(ct);
    }
}
