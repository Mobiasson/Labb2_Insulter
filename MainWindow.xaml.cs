using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Insulter.Model;
using Insulter.Dialog;
using Insulter.Services;
using Microsoft.EntityFrameworkCore;

namespace Insulter;

public partial class MainWindow : INotifyPropertyChanged {
    private string? _insult = "Click 'Insult Me' to get roasted!";
    public string LoadingMessage { get; set; } = "Please wait...";
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

    private void FetchInsult_Click(object sender, RoutedEventArgs e) {
        FetchInsultDialog fid = new FetchInsultDialog();
        fid.ShowDialog();
    }

    private async void GetInsultFromDB_Click(object sender, RoutedEventArgs e) {
        try {
            IsLoading = true;
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

    private async void ClearDatabase_Click(object sender, RoutedEventArgs e) {
        try {
            if(MessageBox.Show("Are you sure you want to delete everything from the database?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes) {
                IsLoading = true;
                LoadingMessage = "Clearing database...";
                await using var db = new InsultContext();
                var deleted = await db.Insults.ExecuteDeleteAsync();
                Insult = deleted > 0 ? $"Deleted {deleted} insults." : "Database was already empty.";
                MessageBox.Show(Insult, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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