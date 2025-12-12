using System.Windows;
using System.Threading.Tasks;
using Insulter.Services;
using Insulter.Model;
using Microsoft.EntityFrameworkCore;

namespace Insulter;
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
    }

    private async void FetchInsult_Click(object sender, RoutedEventArgs e) {
        try {
            var insult = await InsultService.GetInsultObjectAsync();
            if (insult is null) {
                MessageBox.Show("No response", "API", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var db = new InsultContext();
            await db.Database.MigrateAsync();
            db.Insults.Add(insult);
            await db.SaveChangesAsync();

            MessageBox.Show($"Saved #: {insult.Text}", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (System.Exception ex) {
            MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}