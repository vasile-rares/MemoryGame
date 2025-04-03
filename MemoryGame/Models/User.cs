using System;

namespace MemoryGame.Models
{
    public class User
    {
        public string Username { get; set; }
        public string ImagePath { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }

        public User()
        {
            Username = string.Empty;
            ImagePath = string.Empty;
            GamesPlayed = 0;
            GamesWon = 0;
        }
    }
} 