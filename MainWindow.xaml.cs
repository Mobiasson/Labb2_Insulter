using System.Windows;
using System.Threading.Tasks;
using Insulter.Services;

namespace Insulter;
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
    }

    private async void FetchInsult_Click(object sender, RoutedEventArgs e) {
        var json = await InsultService.GetRawJsonAsync();
        MessageBox.Show(json ?? "No response", "Raw JSON", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}