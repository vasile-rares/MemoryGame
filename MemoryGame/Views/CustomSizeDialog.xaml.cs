using System;
using System.Windows;

namespace MemoryGame.Views
{
    public partial class CustomSizeDialog : Window
    {
        public int Rows { get; private set; }
        public int Columns { get; private set; }

        public CustomSizeDialog(int currentRows, int currentColumns)
        {
            InitializeComponent();
            
            // Set initial values
            RowsInput.Text = currentRows.ToString();
            ColumnsInput.Text = currentColumns.ToString();
            
            // Focus the rows text box for immediate user input
            Loaded += (s, e) => RowsInput.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Just collect the inputs, validation will be done in ViewModel
            if (TryGetInputValues(out int rows, out int columns))
            {
                Rows = rows;
                Columns = columns;
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool TryGetInputValues(out int rows, out int columns)
        {
            // Just parse the values, no validation
            bool rowsValid = int.TryParse(RowsInput.Text, out rows);
            bool columnsValid = int.TryParse(ColumnsInput.Text, out columns);
            
            if (!rowsValid)
            {
                MessageBox.Show("Valoarea pentru rânduri nu este un număr valid.", "Input invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
                RowsInput.Focus();
                return false;
            }
            
            if (!columnsValid)
            {
                MessageBox.Show("Valoarea pentru coloane nu este un număr valid.", "Input invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
                ColumnsInput.Focus();
                return false;
            }
            
            return true;
        }
    }
} 