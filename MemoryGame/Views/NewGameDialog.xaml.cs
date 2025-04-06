using System;
using System.Windows;

namespace MemoryGame.Views
{
    public partial class NewGameDialog : Window
    {
        public int GameTime { get; private set; }

        public NewGameDialog(int currentGameTime)
        {
            InitializeComponent();
            
            // Set initial value
            TimeInput.Text = currentGameTime.ToString();
            
            // Focus the text box for immediate user input
            Loaded += (s, e) => TimeInput.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateInput()
        {
            if (int.TryParse(TimeInput.Text, out int time) && time > 0)
            {
                GameTime = time;
                return true;
            }
            else
            {
                MessageBox.Show("Timpul trebuie sa fie un numar pozitiv.", "Input invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
                TimeInput.Focus();
                return false;
            }
        }
    }
} 