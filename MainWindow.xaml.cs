using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Insulter.Services;
using Insulter.Model;
using Microsoft.EntityFrameworkCore;

namespace Insulter;
public partial class MainWindow : Window, INotifyPropertyChanged {
    private string? _insult;
    public string? Insult {
        get => _insult;
        set { _insult = value; OnPropertyChanged(); }
    }

    public MainWindow() {
        InitializeComponent();
        DataContext = this;
    }

    private async void FetchInsult_Click(object sender, RoutedEventArgs e) {
        try {
            var insult = await InsultService.GetInsultObjectAsync();
            Insult = insult?.Text ?? "No response";
            if(insult is null) {
                MessageBox.Show("No response", "API", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            using var db = new InsultContext();
            await db.Database.MigrateAsync();
            db.Insults.Add(insult);
            await db.SaveChangesAsync();
            MessageBox.Show($"Saved #: {insult.Text}", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch(System.Exception ex) {
            MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}