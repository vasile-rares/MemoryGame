using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using MemoryGame.Models;

namespace MemoryGame.Services
{
    public class GameSaveService
    {
        private readonly string _saveGameDirectory;
        
        public GameSaveService()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MemoryGame"
            );
            
            _saveGameDirectory = Path.Combine(appDataPath, "SavedGames");
            Directory.CreateDirectory(_saveGameDirectory);
        }
        
        /// <summary>
        /// Saves the current game state for a specific user
        /// </summary>
        public void SaveGame(
            string username, 
            int selectedCategory, 
            int rows, 
            int columns,
            int remainingTimeInSeconds,
            int elapsedTimeInSeconds,
            ObservableCollection<Card> cards)
        {
            // Create a saved game object
            var savedGame = new SavedGame
            {
                Username = username,
                SelectedCategory = selectedCategory,
                Rows = rows,
                Columns = columns,
                RemainingTimeInSeconds = remainingTimeInSeconds,
                ElapsedTimeInSeconds = elapsedTimeInSeconds,
                Cards = cards.Select(c => new SavedCard
                {
                    Id = c.Id,
                    ImagePath = c.ImagePath,
                    IsFlipped = c.IsFlipped,
                    IsMatched = c.IsMatched
                }).ToList()
            };
            
            // Save the game to a file
            string filePath = GetSaveFilePath(username);
            string json = JsonSerializer.Serialize(savedGame, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            File.WriteAllText(filePath, json);
        }
        
        /// <summary>
        /// Loads a saved game for a specific user
        /// </summary>
        public SavedGame LoadGame(string username)
        {
            string filePath = GetSaveFilePath(username);
            
            if (!File.Exists(filePath))
            {
                return null;
            }
            
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<SavedGame>(json);
        }
        
        /// <summary>
        /// Checks if a saved game exists for a specific user
        /// </summary>
        public bool HasSavedGame(string username)
        {
            string filePath = GetSaveFilePath(username);
            return File.Exists(filePath);
        }
        
        /// <summary>
        /// Gets the save file path for a specific user
        /// </summary>
        private string GetSaveFilePath(string username)
        {
            return Path.Combine(_saveGameDirectory, $"{username}.json");
        }
    }
} 