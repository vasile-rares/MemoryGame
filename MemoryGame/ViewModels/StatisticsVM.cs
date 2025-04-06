using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MemoryGame.Commands;
using MemoryGame.Models;
using MemoryGame.Services;

namespace MemoryGame.ViewModels
{
    public class StatisticsVM : BaseVM
    {
        private readonly UserService _userService;
        private ObservableCollection<User> _users;

        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public ICommand CloseCommand { get; }

        public StatisticsVM()
        {
            _userService = new UserService();
            LoadUsers();
            
            CloseCommand = new RelayCommand<Window>(ExecuteClose);
        }

        private void LoadUsers()
        {
            var usersList = _userService.LoadUsers();
            Users = new ObservableCollection<User>(usersList);
        }

        private void ExecuteClose(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }
    }
} 