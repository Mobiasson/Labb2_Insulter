using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Insulter.Services;

namespace Insulter.Dialog;

public partial class FetchInsultDialog : Window, INotifyPropertyChanged {
    private bool _isLoading;
    public bool IsLoading {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsNotLoading)); }
    }
    public bool IsNotLoading => !IsLoading;

    private string _loadingMessage = "Please wait...";
    public string LoadingMessage {
        get => _loadingMessage;
        set { _loadingMessage = value; OnPropertyChanged(); }
    }

    public ObservableCollection<string> Languages { get; } = new ObservableCollection<string> {
        "en","es","fr","de","it"
    };

    private string _selectedLanguage = "en";
    public string SelectedLanguage {
        get => _selectedLanguage;
        set { _selectedLanguage = value; OnPropertyChanged(); }
    }

    private string _amount = "50";
    public string Amount {
        get => _amount;
        set { _amount = value; OnPropertyChanged(); }
    }

    public FetchInsultDialog() {
        InitializeComponent();
        DataContext = this;
    }

    private async void Fetch_Click(object sender, RoutedEventArgs e) {
        if(!int.TryParse(Amount, out var amt) || amt <= 0) {
            MessageBox.Show("Must be positive.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try {
            LoadingMessage = "Fetching insults...";
            IsLoading = true;
            await Task.Yield();
            var saved = await InsultService.FetchAndSaveBatchAsync(amt, SelectedLanguage);
            MessageBox.Show($"Fetched and saved {saved} insults.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }
        catch(Exception ex) {
            MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            MessageBox.Show($"Sometimes the API is slow to respond. Try again.");
        }
        finally {
            IsLoading = false;
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) {
        this.DialogResult = false;
        this.Close();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}