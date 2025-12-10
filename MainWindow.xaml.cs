using System.Windows;
using Insulter.Model;

namespace Insulter;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        using var db = new InsultContext();
        db.Database.EnsureCreated();
        MessageBox.Show("Database 'insulter' and table 'Insults' are ready!", "Success", MessageBoxButton.OK, MessageBoxImage.Information); // Remove later and add to GUI
    }
}