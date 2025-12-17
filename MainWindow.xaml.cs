using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Insulter.Services;

namespace Insulter;
public partial class MainWindow : Window, INotifyPropertyChanged {
    private string? _insult;
    public string? Insult {
        get => _insult;
        set { _insult = value; OnPropertyChanged(); }
    }

    private bool _isLoading;
    public bool IsLoading {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); }
    }

    public MainWindow() {
        InitializeComponent();
        DataContext = this;
    }

    private async void FetchInsult_Click(object sender, RoutedEventArgs e) {
        try {
            IsLoading = true;
            var saved = await InsultService.FetchAndSaveBatchAsync(50);
            Insult = $"Seeded {saved} insults.";
            MessageBox.Show(Insult, "Batch Seed", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch(System.Exception ex) {
            MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally {
            IsLoading = false;
        }
    }

    private async void GetInsultFromDB_Click(object sender, RoutedEventArgs e) {

    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}