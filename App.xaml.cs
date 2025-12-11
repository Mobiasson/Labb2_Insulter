using System.Windows;
using Insulter.Model;
using Microsoft.EntityFrameworkCore;

namespace Insulter;

public partial class App : Application {
    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);
        try {
            using var db = new InsultContext();
#if DEBUG
                // Dev/test only: drop and recreate DB from migrations each run
                db.Database.EnsureDeleted();
                db.Database.Migrate();
#else
                // Prod: apply incremental migrations only
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
