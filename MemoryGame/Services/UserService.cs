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

        public User UpdateUserStatistics(string username, bool gameWon)
        {
            var users = LoadUsers();
            var user = users.Find(u => u.Username == username);

            if (user != null)
            {
                user.GamesPlayed++;
                if (gameWon)
                {
                    user.GamesWon++;
                }

                SaveUsers(users);
            }

            return user;
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
            if (user == null)
                return;

            if (File.Exists(user.ImagePath))
            {
                try
                {
                    // Force garbage collection to help release file handles
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    File.Delete(user.ImagePath);
                }
                catch (IOException ex)
                {
                    // If we can't delete the file now, mark it for deletion on application exit
                    try
                    {
                        string tempPath = user.ImagePath + ".tobedeleted";
                        if (File.Exists(tempPath))
                            File.Delete(tempPath);
                        File.Move(user.ImagePath, tempPath);
                        user.ImagePath = tempPath;
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine($"Could not delete or rename user image: {ex.Message}");
                    }
                }
            }

            // Delete user's saved games
            var userGamesDirectory = Path.Combine(
                Path.GetDirectoryName(_usersFilePath),
                "SavedGames",
                user.Username
            );
            if (Directory.Exists(userGamesDirectory))
            {
                try
                {
                    Directory.Delete(userGamesDirectory, true);
                }
                catch (IOException)
                {
                }
            }

            // Delete user's statistics
            var statsFilePath = Path.Combine(
                Path.GetDirectoryName(_usersFilePath),
                "Statistics",
                $"{user.Username}.json"
            );
            if (File.Exists(statsFilePath))
            {
                try
                {
                    File.Delete(statsFilePath);
                }
                catch (IOException)
                {
                }
            }
        }
    }
}