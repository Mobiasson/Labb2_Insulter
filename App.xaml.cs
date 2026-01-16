using System;
using System.Windows;
using Insulter.Model;
using Insulter.Dialog;
using Microsoft.EntityFrameworkCore;

namespace Insulter;

public partial class App : Application {
    protected override async void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);

        try {
            using var db = new InsultContext();
            await db.Database.MigrateAsync();
        }
        catch(Exception ex) {
            MessageBox.Show(ex.ToString(), "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
            return;
        }
        var mainWindow = new MainWindow();
        mainWindow.Show();
        var userPrompt = new UserPromptDialog { Owner = mainWindow };
        var result = userPrompt.ShowDialog();

        if(result == true) {
            var user = userPrompt.UserName;
            mainWindow.Title = string.IsNullOrWhiteSpace(user)
                ? "Insulter"
                : $"Insulter - {user}";
        } else {
            mainWindow.Close();
            Shutdown();
        }
    }
}
