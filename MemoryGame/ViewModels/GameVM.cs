using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MemoryGame.Commands;
using MemoryGame.Models;
using MemoryGame.Services;
using MemoryGame.Views;

namespace MemoryGame.ViewModels
{
    public class GameVM : BaseVM
    {
        private int _selectedRows = 4;
        private int _selectedColumns = 4;
        private int _activeRows = 4;
        private int _activeColumns = 4;
        private ObservableCollection<Card> _cards;
        private int _selectedCategory = 1;
        private string _remainingTimeDisplay;
        private User _currentUser;

        // Services
        private readonly GameService _gameService;

        private readonly GameTimerService _timerService;
        private readonly DialogVM _dialogViewModel;
        private readonly GameSaveService _gameSaveService;

        public int SelectedRows
        {
            get => _selectedRows;
            set => SetProperty(ref _selectedRows, value);
        }

        public int SelectedColumns
        {
            get => _selectedColumns;
            set => SetProperty(ref _selectedColumns, value);
        }

        public int ActiveRows
        {
            get => _activeRows;
            set => SetProperty(ref _activeRows, value);
        }

        public int ActiveColumns
        {
            get => _activeColumns;
            set => SetProperty(ref _activeColumns, value);
        }

        public ObservableCollection<Card> Cards
        {
            get => _cards;
            set => SetProperty(ref _cards, value);
        }

        public int SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public int GameTimeInSeconds
        {
            get => _timerService.GameTimeInSeconds;
            set
            {
                _timerService.GameTimeInSeconds = value;
                OnPropertyChanged();
            }
        }

        public string RemainingTimeDisplay
        {
            get => _remainingTimeDisplay;
            set => SetProperty(ref _remainingTimeDisplay, value);
        }

        public bool IsGameInProgress => _gameService.IsGameInProgress;

        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        // Commands
        public ICommand SelectCategoryCommand { get; }

        public ICommand NewGameCommand { get; }
        public ICommand OpenGameCommand { get; }
        public ICommand SaveGameCommand { get; }
        public ICommand StatisticsCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand SetBoardSizeCommand { get; }
        public ICommand AboutCommand { get; }
        public ICommand SelectCardCommand { get; }

        public GameVM()
        {
            _gameService = new GameService();
            _timerService = new GameTimerService();
            _dialogViewModel = new DialogVM();
            _gameSaveService = new GameSaveService();

            // Initialize cards collection
            Cards = new ObservableCollection<Card>();

            // Initialize commands
            SelectCategoryCommand = new RelayCommand<object>(ExecuteSelectCategory);
            NewGameCommand = new RelayCommand(ExecuteNewGame);
            OpenGameCommand = new RelayCommand(ExecuteOpenGame);
            SaveGameCommand = new RelayCommand(ExecuteSaveGame);
            StatisticsCommand = new RelayCommand(ExecuteStatistics);
            ExitCommand = new RelayCommand<object>(ExecuteExit);
            SetBoardSizeCommand = new RelayCommand<object>(ExecuteSetBoardSize);
            AboutCommand = new RelayCommand(ExecuteAbout);
            SelectCardCommand = new RelayCommand<object>(ExecuteSelectCard);

            // Set up timer tick handler
            _timerService.TimerTick += UpdateTimerDisplay;
        }

        public GameVM(User user) : this()
        {
            CurrentUser = user;

            if (_gameSaveService.HasSavedGame(user.Username))
            {
                if (_dialogViewModel.ShowYesNoDialog(
                    "You have a saved game. Do you want to load it?",
                    "Load Saved Game"))
                {
                    ExecuteOpenGame();
                }
            }
        }

        #region Game Commands

        private void UpdateTimerDisplay()
        {
            RemainingTimeDisplay = _timerService.GetTimeDisplay();
            OnPropertyChanged(nameof(IsGameInProgress));

            if (_timerService.RemainingTimeInSeconds <= 0)
            {
                EndGame(false);
            }
        }

        private void FlipCard(Card card)
        {
            int cardIndex = Cards.IndexOf(card);
            if (cardIndex >= 0)
            {
                Card updatedCard = Cards[cardIndex];
                Cards.RemoveAt(cardIndex);
                Cards.Insert(cardIndex, updatedCard);
            }
        }

        private void StartNewGame()
        {
            ActiveRows = SelectedRows;
            ActiveColumns = SelectedColumns;

            Cards = _gameService.CreateGameBoard(SelectedRows, SelectedColumns, SelectedCategory);

            _gameService.StartGame();
            _timerService.Start();

            OnPropertyChanged(nameof(IsGameInProgress));
        }

        private void EndGame(bool isWon)
        {
            _timerService.Stop();
            _gameService.EndGame(isWon);
            OnPropertyChanged(nameof(IsGameInProgress));

            // Update player statistics
            if (CurrentUser != null)
            {
                var userService = new UserService();
                User updatedUser = userService.UpdateUserStatistics(CurrentUser.Username, isWon);

                if (updatedUser != null)
                {
                    CurrentUser = updatedUser;
                }
            }

            if (_dialogViewModel.ShowGameEndDialog(isWon))
            {
                ExecuteNewGame();
            }
        }

        private void ExecuteSelectCard(object parameter)
        {
            if (parameter is Card selectedCard)
            {
                _gameService.SelectCard(selectedCard, FlipCard, EndGame);
            }
        }

        #endregion Game Commands

        #region File

        private void ExecuteSelectCategory(object parameter)
        {
            SelectedCategory = int.Parse(parameter.ToString());

            string categoryName;
            switch (SelectedCategory)
            {
                case 1:
                    categoryName = "Animals";
                    break;

                case 2:
                    categoryName = "Flowers";
                    break;

                case 3:
                    categoryName = "Fruits";
                    break;

                default:
                    categoryName = $"Unknown ({SelectedCategory})";
                    break;
            }

            _dialogViewModel.ShowMessage($"Category {categoryName} selected", "Category Selection");
        }

        private void ExecuteNewGame()
        {
            int timeInSeconds = GameTimeInSeconds;

            if (_dialogViewModel.ShowNewGameDialog(ref _selectedCategory, ref timeInSeconds))
            {
                GameTimeInSeconds = timeInSeconds;
                StartNewGame();
            }
        }

        private void ExecuteOpenGame()
        {
            if (CurrentUser == null)
            {
                _dialogViewModel.ShowMessage("No user is logged in.", "Error", true);
                return;
            }

            if (!_gameSaveService.HasSavedGame(CurrentUser.Username))
            {
                _dialogViewModel.ShowMessage("No saved game found for the current user.", "No Saved Game");
                return;
            }

            // Load the saved game
            Game savedGame = _gameSaveService.LoadGame(CurrentUser.Username);
            if (savedGame == null)
            {
                _dialogViewModel.ShowMessage("Failed to load saved game.", "Error", true);
                return;
            }

            SelectedCategory = savedGame.SelectedCategory;
            SelectedRows = savedGame.Rows;
            SelectedColumns = savedGame.Columns;
            ActiveRows = savedGame.Rows;
            ActiveColumns = savedGame.Columns;

            GameTimeInSeconds = savedGame.RemainingTimeInSeconds;

            ObservableCollection<Card> restoredCards = new ObservableCollection<Card>();
            foreach (var card in savedGame.Cards)
            {
                card.IsFlipped = false;
                restoredCards.Add(card);
            }
            Cards = restoredCards;

            _gameService.StartGame(Cards);
            _timerService.Start();

            OnPropertyChanged(nameof(IsGameInProgress));

            _dialogViewModel.ShowMessage("Game loaded successfully!", "Game Loaded");
        }

        private void ExecuteSaveGame()
        {
            if (!IsGameInProgress)
            {
                _dialogViewModel.ShowMessage("No game in progress to save.", "Save Game");
                return;
            }

            if (CurrentUser == null)
            {
                _dialogViewModel.ShowMessage("No user is logged in.", "Error", true);
                return;
            }

            int elapsedTimeInSeconds = GameTimeInSeconds - _timerService.RemainingTimeInSeconds;

            _gameSaveService.SaveGame(
                CurrentUser.Username,
                SelectedCategory,
                ActiveRows,
                ActiveColumns,
                _timerService.RemainingTimeInSeconds,
                elapsedTimeInSeconds,
                Cards
            );

            _dialogViewModel.ShowMessage("Game saved successfully!", "Game Saved");
        }

        private void ExecuteStatistics()
        {
            var statisticsWindow = new StatisticsWindow();
            statisticsWindow.Owner = Application.Current.MainWindow;
            statisticsWindow.ShowDialog();
        }

        private void ExecuteExit(object parameter)
        {
            if (_timerService.IsRunning())
            {
                _timerService.Stop();
            }

            if (parameter is Window paramWindow)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                paramWindow.Close();
            }
            else
            {
                foreach (Window currentWindow in Application.Current.Windows)
                {
                    if (currentWindow is GameWindow gameWindow)
                    {
                        var loginWindow = new LoginWindow();
                        loginWindow.Show();
                        gameWindow.Close();
                        break;
                    }
                }
            }
        }

        #endregion File

        #region Options

        private void ExecuteSetBoardSize(object parameter)
        {
            string size = parameter.ToString();
            if (size == "standard")
            {
                SelectedRows = 4;
                SelectedColumns = 4;
                _dialogViewModel.ShowMessage("Standard 4x4 board selected. Changes will apply when you start a new game.", "Board Size");
            }
            else if (size == "custom")
            {
                int rows = SelectedRows;
                int columns = SelectedColumns;

                if (_dialogViewModel.ShowCustomSizeDialog(ref rows, ref columns))
                {
                    SelectedRows = rows;
                    SelectedColumns = columns;
                    _dialogViewModel.ShowMessage($"Custom {SelectedRows}x{SelectedColumns} board selected. Changes will apply when you start a new game.", "Board Size");
                }
            }
        }

        #endregion Options

        #region About

        private void ExecuteAbout()
        {
            _dialogViewModel.ShowMessage(
                "Memory Game\n\n" +
                "Student: Rares-Mihail Vasile\n" +
                "Email: rares-mihail.vasile@student.unitbv.ro\n" +
                "Group: 10LF234\n" +
                "Specialization: Mate-Info",
                "About");
        }

        #endregion About
    }
}