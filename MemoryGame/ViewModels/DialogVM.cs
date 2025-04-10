using System;
using System.Windows;
using MemoryGame.Views;

namespace MemoryGame.ViewModels
{
    public class DialogVM : BaseVM
    {
        public bool ShowCustomSizeDialog(ref int rows, ref int columns)
        {
            CustomSizeDialog dialog = new CustomSizeDialog(rows, columns);

            // Seteaza proprietatea Owner la fereastra activa
            if (Application.Current.MainWindow != null)
            {
                dialog.Owner = Application.Current.MainWindow;
            }

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                if (ValidateBoardSize(dialog.Rows, dialog.Columns, out string errorMessage))
                {
                    rows = dialog.Rows;
                    columns = dialog.Columns;
                    return true;
                }
                else
                {
                    ShowMessage(errorMessage, "Dimensiune invalida", true);
                    return ShowCustomSizeDialog(ref rows, ref columns);
                }
            }

            return false;
        }

        private bool ValidateBoardSize(int rows, int columns, out string errorMessage)
        {
            if (rows < 2 || rows > 6)
            {
                errorMessage = "Numarul de randuri trebuie sa fie intre 2 si 6.";
                return false;
            }

            if (columns < 2 || columns > 6)
            {
                errorMessage = "Numarul de coloane trebuie sa fie intre 2 si 6.";
                return false;
            }

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
            NewGameDialog dialog = new NewGameDialog(timeInSeconds);

            // Seteaza proprietatea Owner la fereastra activa
            if (Application.Current.MainWindow != null)
            {
                dialog.Owner = Application.Current.MainWindow;
            }

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