using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using MemoryGame.Models;

namespace MemoryGame.Services
{
    public class UserService
    {
        private readonly string _usersFilePath;
        private readonly string _userImagesDirectory;

        public UserService()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MemoryGame"
            );
            
            _usersFilePath = Path.Combine(appDataPath, "users.json");
            _userImagesDirectory = Path.Combine(appDataPath, "UserImages");

            Directory.CreateDirectory(appDataPath);
            Directory.CreateDirectory(_userImagesDirectory);
        }

        public List<User> LoadUsers()
        {
            if (!File.Exists(_usersFilePath))
            {
                return new List<User>();
            }

            var json = File.ReadAllText(_usersFilePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        public void SaveUsers(List<User> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            File.WriteAllText(_usersFilePath, json);
        }

        public string SaveUserImage(string sourceImagePath, string username)
        {
            var extension = Path.GetExtension(sourceImagePath);
            var newFileName = $"{username}{extension}";
            var destinationPath = Path.Combine(_userImagesDirectory, newFileName);

            File.Copy(sourceImagePath, destinationPath, true);
            return destinationPath;
        }

        public void DeleteUser(User user)
        {
            if (user == null) return;

            // Delete user's image file
            if (File.Exists(user.ImagePath))
            {
                File.Delete(user.ImagePath);
            }

            // Delete user's saved games
            var userGamesDirectory = Path.Combine(
                Path.GetDirectoryName(_usersFilePath),
                "SavedGames",
                user.Username
            );
            if (Directory.Exists(userGamesDirectory))
            {
                Directory.Delete(userGamesDirectory, true);
            }

            // Delete user's statistics
            var statsFilePath = Path.Combine(
                Path.GetDirectoryName(_usersFilePath),
                "Statistics",
                $"{user.Username}.json"
            );
            if (File.Exists(statsFilePath))
            {
                File.Delete(statsFilePath);
            }
        }
    }
} 