using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Win32;
using MemoryGame.Models;
using MemoryGame.Services;
using MemoryGame.Commands;
using System.Collections.Generic;
using System.Windows;
using MemoryGame.Views;

namespace MemoryGame.ViewModels
{
    public class LoginVM : BaseVM
    {
        private readonly UserService _userService;
        private ObservableCollection<User> _users;
        private User _selectedUser;
        private string _newUsername;
        private string _newUserImagePath;
        private bool _isDeleteEnabled;
        private bool _isPlayEnabled;

        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (SetProperty(ref _selectedUser, value))
                {
                    IsDeleteEnabled = value != null;
                    IsPlayEnabled = value != null;
                }
            }
        }

        public string NewUsername
        {
            get => _newUsername;
            set => SetProperty(ref _newUsername, value);
        }

        public string NewUserImagePath
        {
            get => _newUserImagePath;
            set => SetProperty(ref _newUserImagePath, value);
        }

        public bool IsDeleteEnabled
        {
            get => _isDeleteEnabled;
            set => SetProperty(ref _isDeleteEnabled, value);
        }

        public bool IsPlayEnabled
        {
            get => _isPlayEnabled;
            set => SetProperty(ref _isPlayEnabled, value);
        }

        public ICommand CreateUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand SelectImageCommand { get; }

        public LoginVM()
        {
            _userService = new UserService();
            Users = new ObservableCollection<User>(_userService.LoadUsers());

            CreateUserCommand = new RelayCommand(CreateUser);
            DeleteUserCommand = new RelayCommand(DeleteUser);
            PlayCommand = new RelayCommand(Play);
            SelectImageCommand = new RelayCommand(SelectImage);
        }

        private void CreateUser()
        {
            if (string.IsNullOrWhiteSpace(NewUsername) || string.IsNullOrWhiteSpace(NewUserImagePath)) return;

            var savedImagePath = _userService.SaveUserImage(NewUserImagePath, NewUsername);
            var newUser = new User
            {
                Username = NewUsername,
                ImagePath = savedImagePath
            };

            Users.Add(newUser);
            _userService.SaveUsers(new List<User>(Users));

            NewUsername = string.Empty;
            NewUserImagePath = string.Empty;
        }

        private void DeleteUser()
        {
            if (SelectedUser == null) return;

            // Make a local copy to avoid reference issues
            var userToDelete = SelectedUser;

            // Clear the selected user first to release any image references
            SelectedUser = null;

            // Force garbage collection to release file handles before deletion
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();

            // Now delete the user
            _userService.DeleteUser(userToDelete);
            Users.Remove(userToDelete);
            _userService.SaveUsers(new List<User>(Users));
        }

        private void Play()
        {
            if (SelectedUser == null)
                return;

            // Open the game window with the selected user
            var gameWindow = new GameWindow(SelectedUser);
            gameWindow.Show();

            // Close the login window
            foreach (Window window in Application.Current.Windows)
            {
                if (window is LoginWindow loginWindow)
                {
                    loginWindow.Close();
                    break;
                }
            }
        }

        private void SelectImage()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif)|*.jpg;*.jpeg;*.png;*.gif"
            };

            if (dialog.ShowDialog() == true)
            {
                NewUserImagePath = dialog.FileName;
            }
        }
    }
}