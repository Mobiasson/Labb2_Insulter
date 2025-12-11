using System.Windows;
using System.Threading.Tasks;
using Insulter.Model;

namespace Insulter;
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
    }

    private async void FetchInsult_Click(object sender, RoutedEventArgs e) {
        var insult = await InsultService.GetInsultAsync();
        MessageBox.Show(insult ?? "No insult received", "API Test", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}