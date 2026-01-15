using Insulter.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace Insulter.Dialog;

public partial class UserPromptDialog : Window, INotifyPropertyChanged {
    private string? _userName;


    public string? UserName {
        get => _userName;
        private set { _userName = value; OnPropertyChanged(); }
    }

    public int? CreatedUserId { get; private set; }

    public UserPromptDialog() {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Loaded += (_, _) => NameTextBox.Focus();
    }

    private async void Ok_Click(object sender, RoutedEventArgs e) {
        var name = NameTextBox.Text?.Trim();
        bool hasDigits = name.Any(char.IsDigit);
        if(string.IsNullOrEmpty(name)) {
            MessageBox.Show("Username cannot be null.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            NameTextBox.Focus();
            return;
        } else if(hasDigits) {
            MessageBox.Show("UserName cannot contains digits");
            return;
        }

        try {
            await using var db = new InsultContext();
            var existing = await db.Users.FirstOrDefaultAsync(u => u.Name == name);
            if(existing is not null) {
                UserName = existing.Name;
                CreatedUserId = existing.Id;
                DialogResult = true;
                Close();
                return;
            }
            var user = new User { Name = name };
            db.Users.Add(user);
            await db.SaveChangesAsync();
            UserName = name;
            CreatedUserId = user.Id;
            DialogResult = true;
            Close();
        }
        catch(Exception ex) {
            MessageBox.Show(ex.ToString(), "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) {
        DialogResult = false;
        Close();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}