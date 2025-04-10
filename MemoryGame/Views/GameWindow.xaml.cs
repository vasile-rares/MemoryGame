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
            this.DataContext = new GameVM();
        }

        public GameWindow(User user)
        {
            InitializeComponent();
            this.DataContext = new GameVM(user);
        }
    }
}