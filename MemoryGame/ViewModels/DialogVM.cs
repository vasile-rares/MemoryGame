using System;
using System.Windows;
using MemoryGame.Views;

namespace MemoryGame.ViewModels
{
    public class DialogVM : BaseVM
    {
        public bool ShowCustomSizeDialog(ref int rows, ref int columns)
        {
            // Crează dialogul cu valorile curente
            CustomSizeDialog dialog = new CustomSizeDialog(rows, columns);

            // Setează proprietatea Owner la fereastra activă
            if (Application.Current.MainWindow != null)
            {
                dialog.Owner = Application.Current.MainWindow;
            }

            // Afișează dialogul și procesează rezultatul
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                // Validare valori introduse
                if (ValidateBoardSize(dialog.Rows, dialog.Columns, out string errorMessage))
                {
                    rows = dialog.Rows;
                    columns = dialog.Columns;
                    return true;
                }
                else
                {
                    // Afiseaza mesajul de eroare
                    ShowMessage(errorMessage, "Dimensiune invalida", true);

                    // Redeschide dialogul
                    return ShowCustomSizeDialog(ref rows, ref columns);
                }
            }

            return false;
        }

        private bool ValidateBoardSize(int rows, int columns, out string errorMessage)
        {
            // Verifica daca randurile sunt in intervalul corect
            if (rows < 2 || rows > 6)
            {
                errorMessage = "Numarul de randuri trebuie sa fie intre 2 si 6.";
                return false;
            }

            // Verifica daca coloanele sunt in intervalul corect
            if (columns < 2 || columns > 6)
            {
                errorMessage = "Numarul de coloane trebuie sa fie intre 2 si 6.";
                return false;
            }

            // Verifica daca produsul este par
            if ((rows * columns) % 2 != 0)
            {
                errorMessage = "Produsul randuri × coloane trebuie sa fie un numar par.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        public bool ShowNewGameDialog(ref int categoryId, ref int timeInSeconds)
        {
            // Creeaza dialogul cu timpul curent
            NewGameDialog dialog = new NewGameDialog(timeInSeconds);

            // Seteaza proprietatea Owner la fereastra activa
            if (Application.Current.MainWindow != null)
            {
                dialog.Owner = Application.Current.MainWindow;
            }

            // Afiseaza dialogul si proceseaza rezultatul
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                timeInSeconds = dialog.GameTime;
                return true;
            }

            return false;
        }

        public bool ShowGameEndDialog(bool isWon)
        {
            string message = isWon
                ? "Felicitari! Ai gasit toate perechile!"
                : "Timpul a expirat! Jocul s-a terminat.";

            message += "\n\nVrei sa joci din nou?";

            string title = isWon ? "Joc castigat" : "Joc pierdut";

            MessageBoxResult result = MessageBox.Show(
                message, 
                title, 
                MessageBoxButton.YesNo,
                isWon ? MessageBoxImage.Information : MessageBoxImage.Warning);
                
            return result == MessageBoxResult.Yes;
        }

        public void ShowMessage(string message, string title, bool isWarning = false)
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButton.OK,
                isWarning ? MessageBoxImage.Warning : MessageBoxImage.Information
            );
        }

        public bool ShowYesNoDialog(string message, string title)
        {
            MessageBoxResult result = MessageBox.Show(
                message,
                title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            return result == MessageBoxResult.Yes;
        }
    }
}