using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Win32;
using MemoryGame.Models;
using MemoryGame.Services;

namespace MemoryGame.ViewModels
{
    public class LoginViewModel : ViewModelBase
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

        public LoginViewModel()
        {
            _userService = new UserService();
            Users = new ObservableCollection<User>(_userService.LoadUsers());
            
            CreateUserCommand = new RelayCommand(CreateUser, CanCreateUser);
            DeleteUserCommand = new RelayCommand(DeleteUser, () => SelectedUser != null);
            PlayCommand = new RelayCommand(Play, () => SelectedUser != null);
            SelectImageCommand = new RelayCommand(SelectImage);
        }

        private bool CanCreateUser()
        {
            return !string.IsNullOrWhiteSpace(NewUsername) && !string.IsNullOrWhiteSpace(NewUserImagePath);
        }

        private void CreateUser()
        {
            if (!CanCreateUser()) return;

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

            _userService.DeleteUser(SelectedUser);
            Users.Remove(SelectedUser);
            _userService.SaveUsers(new List<User>(Users));
            SelectedUser = null;
        }

        private void Play()
        {
            // TODO: Navigate to game window
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