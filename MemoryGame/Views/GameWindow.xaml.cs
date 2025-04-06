using System.Windows;
using MemoryGame.Models;
using MemoryGame.ViewModels;

namespace MemoryGame.Views
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public GameWindow()
        {
            InitializeComponent();
            // Create a default GameVM instance
            this.DataContext = new GameVM();
        }
        
        public GameWindow(User user)
        {
            InitializeComponent();
            // Create a GameVM with the user and set it as DataContext
            this.DataContext = new GameVM(user);
        }
    }
} 