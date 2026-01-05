namespace Insulter.Services;

using Microsoft.Extensions.Configuration;
using NAudio.Wave;
using Insulter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Speech.Synthesis;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class VoiceService {
    private readonly HttpClient _http = new HttpClient();
    private readonly SpeechSynthesizer _windowsTts = new SpeechSynthesizer();

    public VoiceService(IConfiguration config) {
    }
    public async Task<List<FakeYouVoice>> GetAllVoicesAsync() {
        var response = await _http.GetAsync("https://api.fakeyou.com/tts/list");
        if(!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Failed to load voices: {response.StatusCode}");
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        if(!doc.RootElement.GetProperty("success").GetBoolean())
            throw new Exception("FakeYou API returned success=false");
        var models = doc.RootElement.GetProperty("models").EnumerateArray();
        var voices = new List<FakeYouVoice>();
        foreach(var model in models) {
            voices.Add(new FakeYouVoice {
                ModelToken = model.GetProperty("model_token").GetString() ?? "",
                Title = model.GetProperty("title").GetString() ?? "Unknown",
                CreatorUsername = model.GetProperty("creator_username").GetString() ?? "Unknown"
            });
        }
        return voices.OrderBy(v => v.Title).ToList();
    }

    // Speak the text – tries FakeYou first (WAV file), falls back to Windows TTS
    public async Task SpeakAsync(string text, string? fakeYouModelToken = null) {
        if(!string.IsNullOrWhiteSpace(fakeYouModelToken)) {
            bool success = await TryFakeYouAsync(text, fakeYouModelToken);
            if(success) return;
        }

        FallbackToWindowsTts(text);
    }

    private async Task<bool> TryFakeYouAsync(string text, string modelToken) {
        try {
            var requestBody = new {
                uuid_idempotency_token = Guid.NewGuid().ToString(),
                tts_model_token = modelToken,
                inference_text = text
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync("https://api.fakeyou.com/tts/inference", content);
            if(!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            string jobToken = result.GetProperty("inference_job_token").GetString()!;

            string? audioPath = await PollJobAsync(jobToken);
            if(audioPath != null) {
                await PlayWithNAudioAsync($"https://storage.googleapis.com/vocodes-public{audioPath}");
                return true;
            }
        }
        catch {
            // Silently fall back to Windows TTS
        }

        return false;
    }

    private async Task<string?> PollJobAsync(string jobToken) {
        string url = $"https://api.fakeyou.com/tts/job/{jobToken}";

        for(int i = 0; i < 60; i++) // Max ~30 seconds
        {
            await Task.Delay(500);

            var resp = await _http.GetAsync(url);
            if(!resp.IsSuccessStatusCode) continue;

            var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
            string status = json.GetProperty("state").GetProperty("status").GetString()!;

            if(status == "complete_success") {
                return json.GetProperty("state")
                           .GetProperty("maybe_public_bucket_wav_audio_path")
                           .GetString();
            }

            if(status.StartsWith("dead") || status.StartsWith("failed"))
                return null;
        }

        return null; // Timeout
    }

    private async Task PlayWithNAudioAsync(string audioUrl) {
        var bytes = await _http.GetByteArrayAsync(audioUrl);
        using var ms = new MemoryStream(bytes);
        using var rdr = new WaveFileReader(ms);     // Changed: FakeYou returns WAV, not MP3
        using var wo = new WaveOutEvent();

        wo.Init(rdr);
        wo.Play();

        while(wo.PlaybackState == PlaybackState.Playing)   // Correct spelling
            await Task.Delay(100);
    }

    private void FallbackToWindowsTts(string text) {
        try {
            var englishVoices = _windowsTts.GetInstalledVoices()
                                           .Select(v => v.VoiceInfo)
                                           .Where(v => v.Culture.TwoLetterISOLanguageName == "en")
                                           .ToList();

            if(englishVoices.Any()) {
                var randomVoice = englishVoices[new Random().Next(englishVoices.Count)];
                _windowsTts.SelectVoice(randomVoice.Name);
            }

            _windowsTts.Rate = 0;
            _windowsTts.Volume = 100;
            _windowsTts.Speak(text);
        }
        catch {
            System.Media.SystemSounds.Beep.Play();
        }
    }
}