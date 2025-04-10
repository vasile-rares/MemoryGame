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

        var loginWindow = new LoginWindow();
        loginWindow.Show();

        this.Close();
    }
}