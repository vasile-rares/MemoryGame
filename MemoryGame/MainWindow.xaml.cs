using System.Windows;
using MemoryGame.Views;

namespace MemoryGame;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Create and show the login window
        var loginWindow = new LoginWindow();
        loginWindow.Show();
        
        // Close the main window
        this.Close();
    }
}