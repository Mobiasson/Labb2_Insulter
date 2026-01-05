using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Insulter.Model;
using Insulter.Services;
using Microsoft.EntityFrameworkCore;

namespace Insulter;

public partial class MainWindow : Window, INotifyPropertyChanged {
    private string? _insult = "Click 'Insult Me' to get roasted!";
    public string? Insult {
        get => _insult;
        set { _insult = value; OnPropertyChanged(); }
    }

    private bool _isLoading;
    public bool IsLoading {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); }
    }

    public string LoadingMessage { get; set; } = "Please wait...";

    // Voice dropdown properties
    public ObservableCollection<Model.FakeYouVoice> AvailableVoices { get; } = new();
    private Model.FakeYouVoice? _selectedVoice;
    public Model.FakeYouVoice? SelectedVoice {
        get => _selectedVoice;
        set { _selectedVoice = value; OnPropertyChanged(); }
    }

    private readonly VoiceService _voiceService = new VoiceService(null!); // Config not needed

    public MainWindow() {
        InitializeComponent();
        DataContext = this;
        // Load voices on startup
        LoadVoicesAsync();
    }

    private async void LoadVoicesAsync() {
        IsLoading = true;
        LoadingMessage = "Loading voices from FakeYou...";
        try {
            var voices = await _voiceService.GetAllVoicesAsync();
            AvailableVoices.Clear();
            foreach(var voice in voices)
                AvailableVoices.Add(voice);

            // Optional: Pre-select a popular one (e.g., first or search for Trump)
            SelectedVoice = AvailableVoices.FirstOrDefault(v => v.Title.Contains("Trump", System.StringComparison.OrdinalIgnoreCase));
        }
        catch(System.Exception ex) {
            MessageBox.Show($"Could not load voices: {ex.Message}\nUsing Windows voices only.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally {
            IsLoading = false;
        }
    }

    private async void FetchInsult_Click(object sender, RoutedEventArgs e) {
        try {
            IsLoading = true;
            LoadingMessage = "Fetching new insults...";
            var saved = await InsultService.FetchAndSaveBatchAsync(50);
            Insult = $"Fetched and saved {saved} new insults!";
            MessageBox.Show(Insult, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch(System.Exception ex) {
            MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally {
            IsLoading = false;
        }
    }

    private async void GetInsultFromDB_Click(object sender, RoutedEventArgs e) {
        try {
            IsLoading = true;
            LoadingMessage = "Getting a random insult...";
            var insult = await InsultService.GetRandomInsultAsync();
            Insult = insult?.Text ?? "No insults in database yet. Click 'Fetch Insults' first!";
        }
        catch(System.Exception ex) {
            MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally {
            IsLoading = false;
        }
    }

    // New: Speak button handler (replace the old Click with this or add Command)
    private async void SpeakInsult_Click(object sender, RoutedEventArgs e) {
        if(string.IsNullOrWhiteSpace(Insult)) {
            MessageBox.Show("No insult to speak!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        IsLoading = true;
        LoadingMessage = "Generating voice (may take 10-30 seconds)...";
        try {
            string? modelToken = SelectedVoice?.ModelToken;
            await _voiceService.SpeakAsync(Insult, modelToken);
        }
        catch(System.Exception ex) {
            MessageBox.Show($"Voice error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally {
            IsLoading = false;
        }
    }

    private async void ClearDatabase_Click(object sender, RoutedEventArgs e) {
        try {
            IsLoading = true;
            LoadingMessage = "Clearing database...";
            await using var db = new InsultContext();
            var deleted = await db.Insults.ExecuteDeleteAsync();
            Insult = deleted > 0 ? $"Deleted {deleted} insults." : "Database was already empty.";
            MessageBox.Show(Insult, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch(System.Exception ex) {
            MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally {
            IsLoading = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}