using System;
using System.Windows;
using Insulter.Model;
using Insulter.Dialog;
using Microsoft.EntityFrameworkCore;

namespace Insulter;

public partial class App : Application {
    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);

        try {
            using var db = new InsultContext();
#if DEBUG
            db.Database.Migrate();
#endif
        }
        catch(Exception ex) {
            MessageBox.Show(ex.ToString(), "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
            return;
        }
        var mainWindow = new MainWindow();
        mainWindow.Show();
        var userPrompt = new UserPromptDialog { Owner = mainWindow };
        var result = userPrompt.ShowDialog();
        if(result == true) {
            return;
        }

        mainWindow.Close();
        Shutdown();
    }
}
