using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Insulter.Model;
using Insulter.Services;
using Microsoft.EntityFrameworkCore;

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
            Insult = $"I found {saved} ways to insult you";
            MessageBox.Show(Insult, "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch(Exception ex) {
            MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally {
            IsLoading = false;
        }
    }

    private async void GetInsultFromDB_Click(object sender, RoutedEventArgs e) {
        try {
            IsLoading = true;
            var insult = await InsultService.GetRandomInsultAsync();
            Insult = insult?.Text ?? "I really want to insult you. But there are no insults in the database.";
        }
        catch(Exception ex) {
            MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally {
            IsLoading = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private async void ClearDatabase_Click(object sender, RoutedEventArgs e) {
        try {
            IsLoading = true;
            await using var db = new InsultContext();
            var deleted = await db.Insults.ExecuteDeleteAsync();
            Insult = deleted > 0 ? $"Deleted {deleted} insults from the database." : "No insults to delete.";
            MessageBox.Show(Insult, "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch(Exception ex) {
            MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally {
            IsLoading = false;
        }
    }
}