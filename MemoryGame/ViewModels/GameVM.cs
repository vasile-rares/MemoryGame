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
        private List<int> _boardSizeOptions = new List<int> { 2, 3, 4, 5, 6 };
        private ObservableCollection<Card> _cards;
        private int _selectedCategory = 1;
        private string _remainingTimeDisplay;
        private bool _isGameEnded;
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

        public List<int> BoardSizeOptions
        {
            get => _boardSizeOptions;
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

        public bool IsGameEnded
        {
            get => _isGameEnded;
            set => SetProperty(ref _isGameEnded, value);
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
            // Initialize services
            _gameService = new GameService();
            _timerService = new GameTimerService();
            _dialogViewModel = new DialogVM();
            _gameSaveService = new GameSaveService();

            // Subscribe to service events
            _gameService.GameEnded += OnGameEnded;
            _timerService.TimeUpdated += OnTimeUpdated;
            _timerService.TimeExpired += OnTimeExpired;

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
        }

        public GameVM(User user) : this()
        {
            CurrentUser = user;
            
            // Check if the user has a saved game
            if (_gameSaveService.HasSavedGame(user.Username))
            {
                // Ask if the user wants to load the saved game
                if (_dialogViewModel.ShowYesNoDialog(
                    "You have a saved game. Do you want to load it?", 
                    "Load Saved Game"))
                {
                    ExecuteOpenGame();
                }
            }
        }

        #region Event Handlers

        private void OnTimeUpdated(object sender, int remainingTime)
        {
            RemainingTimeDisplay = _timerService.GetTimeDisplay();
            OnPropertyChanged(nameof(IsGameInProgress));
        }

        private void OnTimeExpired(object sender, EventArgs e)
        {
            _gameService.EndGame(false);
        }

        private void OnGameEnded(object sender, bool isWon)
        {
            _timerService.Stop();
            IsGameEnded = true;
            OnPropertyChanged(nameof(IsGameInProgress));

            // Update player statistics
            if (CurrentUser != null)
            {
                // Load all users to find the current one
                var userService = new UserService();
                var users = userService.LoadUsers();
                var user = users.FirstOrDefault(u => u.Username == CurrentUser.Username);
                
                if (user != null)
                {
                    // Update statistics
                    user.GamesPlayed++;
                    if (isWon)
                    {
                        user.GamesWon++;
                    }
                    
                    // Save updated statistics
                    userService.SaveUsers(users);
                    
                    // Update current user
                    CurrentUser = user;
                }
            }

            // Show game end dialog and check if player wants to play again
            if (_dialogViewModel.ShowGameEndDialog(isWon))
            {
                ExecuteNewGame();
            }
        }

        #endregion Event Handlers

        #region Game Commands

        private void ExecuteSelectCard(object parameter)
        {
            if (parameter is Card selectedCard)
            {
                _gameService.SelectCard(selectedCard);
            }
        }

        private void StartNewGame()
        {
            // Update active board dimensions to match selected dimensions
            ActiveRows = SelectedRows;
            ActiveColumns = SelectedColumns;
            
            // Reset game ended state
            IsGameEnded = false;

            // Create the game board
            Cards = _gameService.CreateGameBoard(SelectedRows, SelectedColumns, SelectedCategory);

            // Start the game
            _gameService.StartGame();
            _timerService.Start();

            OnPropertyChanged(nameof(IsGameInProgress));
        }

        #endregion Game Commands

        #region File

        private void ExecuteSelectCategory(object parameter)
        {
            SelectedCategory = int.Parse(parameter.ToString());
            
            // Get the category name based on the selected category ID
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

            // Show new game dialog with only time setting
            if (_dialogViewModel.ShowNewGameDialog(ref _selectedCategory, ref timeInSeconds))
            {
                // Apply the time value
                GameTimeInSeconds = timeInSeconds;

                // Start a new game
                StartNewGame();
            }
        }

        private void ExecuteOpenGame()
        {
            // Check if current user is set
            if (CurrentUser == null)
            {
                _dialogViewModel.ShowMessage("No user is logged in.", "Error", true);
                return;
            }

            // Check if there's a saved game for the current user
            if (!_gameSaveService.HasSavedGame(CurrentUser.Username))
            {
                _dialogViewModel.ShowMessage("No saved game found for the current user.", "No Saved Game");
                return;
            }

            // Load the saved game
            SavedGame savedGame = _gameSaveService.LoadGame(CurrentUser.Username);
            if (savedGame == null)
            {
                _dialogViewModel.ShowMessage("Failed to load saved game.", "Error", true);
                return;
            }

            // Update game settings from saved game
            SelectedCategory = savedGame.SelectedCategory;
            SelectedRows = savedGame.Rows;
            SelectedColumns = savedGame.Columns;
            ActiveRows = savedGame.Rows;
            ActiveColumns = savedGame.Columns;
            
            // Set the remaining time
            GameTimeInSeconds = savedGame.RemainingTimeInSeconds;
            
            // Create cards from saved state
            ObservableCollection<Card> restoredCards = new ObservableCollection<Card>();
            foreach (var savedCard in savedGame.Cards)
            {
                restoredCards.Add(new Card
                {
                    Id = savedCard.Id,
                    ImagePath = savedCard.ImagePath,
                    IsFlipped = savedCard.IsFlipped,
                    IsMatched = savedCard.IsMatched
                });
            }
            Cards = restoredCards;
            
            // Start the game
            _gameService.StartGame();
            
            // Start the timer
            _timerService.Start();
            
            // Update UI
            OnPropertyChanged(nameof(IsGameInProgress));
            
            _dialogViewModel.ShowMessage("Game loaded successfully!", "Game Loaded");
        }

        private void ExecuteSaveGame()
        {
            // Check if there's a game in progress
            if (!IsGameInProgress)
            {
                _dialogViewModel.ShowMessage("No game in progress to save.", "Save Game");
                return;
            }
            
            // Check if current user is set
            if (CurrentUser == null)
            {
                _dialogViewModel.ShowMessage("No user is logged in.", "Error", true);
                return;
            }
            
            // Calculate elapsed time
            int elapsedTimeInSeconds = GameTimeInSeconds - _timerService.RemainingTimeInSeconds;
            
            // Save the game
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
            // Create and show the statistics window
            var statisticsWindow = new StatisticsWindow();
            statisticsWindow.Owner = Application.Current.MainWindow;
            statisticsWindow.ShowDialog();
        }

        private void ExecuteExit(object parameter)
        {
            // Stop the timer if it's running
            if (_timerService.IsRunning())
            {
                _timerService.Stop();
            }

            // Logic to exit back to login window
            if (parameter is Window paramWindow)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                paramWindow.Close();
            }
            else
            {
                // Find the current window
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
                // Just store the selected board size without starting a new game
                SelectedRows = 4;
                SelectedColumns = 4;
                _dialogViewModel.ShowMessage("Standard 4x4 board selected. Changes will apply when you start a new game.", "Board Size");
            }
            else if (size == "custom")
            {
                // Show custom size dialog
                int rows = SelectedRows;
                int columns = SelectedColumns;

                if (_dialogViewModel.ShowCustomSizeDialog(ref rows, ref columns))
                {
                    // Just store the selected board size without starting a new game
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
            // Show about information
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