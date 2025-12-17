using System.Windows;
using Insulter.Model;
using Insulter.Services;
using Microsoft.EntityFrameworkCore;

namespace Insulter;

public partial class App : Application {
    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);
        try {
            using var db = new InsultContext();
#if DEBUG
            db.Database.EnsureDeleted();
            db.Database.Migrate();
#endif
        }
        catch(System.Exception ex) {
            MessageBox.Show(ex.ToString(), "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        var window = new MainWindow();
        window.Show();
    }
}
