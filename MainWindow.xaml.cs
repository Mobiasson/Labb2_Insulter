using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Insulter.Model;
using Insulter.Dialog;
using Insulter.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Insulter;

public partial class MainWindow : INotifyPropertyChanged {
    private string? _insult = "Click 'Insult Me' to get roasted!";
    private string? _user;
    public string LoadingMessage { get; set; } = "Please wait...";
    public string? Insult {
        get => _insult;
        set { _insult = value; OnPropertyChanged(); }
    }
    public string? User {
        get => _user;
        set {
            _user = value; OnPropertyChanged();
        }
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

    private async void ClearInsults_Click(object sender, RoutedEventArgs e) {
        try {
            if(MessageBox.Show("Are you sure you wnat to delete all insults in the database?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes) {
                IsLoading = true;
                LoadingMessage = "Clearing database...";
                await using var db = new InsultContext();
                var deleted = await db.Insults.ExecuteDeleteAsync();
                Insult = deleted > 0 ? $"Deleted {deleted} insults. Let me know if you want me find new ways to insult you :)" : "Database was already empty.";
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
    private async void ClearUsers_Click(object sender, RoutedEventArgs e) {
        try {
            if(MessageBox.Show("Are you sure you want to clear all users from the database?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
                IsLoading = true;
                LoadingMessage = "Deleting users from database...";
                await using var db = new InsultContext();
                var deleted = await db.Users.ExecuteDeleteAsync();
                User = deleted > 0 ? $"Deleted {User} users from the database" : "No users in database to delete";
                MessageBox.Show(User, "Deleted all users", MessageBoxButton.OK, MessageBoxImage.Information);
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