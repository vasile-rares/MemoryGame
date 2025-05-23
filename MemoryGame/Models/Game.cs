using System;
using System.Collections.Generic;

namespace MemoryGame.Models
{
    public class Game
    {
        public string Username { get; set; }

        public int SelectedCategory { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

        public int RemainingTimeInSeconds { get; set; }
        public int ElapsedTimeInSeconds { get; set; }

        public List<Card> Cards { get; set; }

        public Game()
        {
            Username = string.Empty;
            SelectedCategory = 1;
            Rows = 4;
            Columns = 4;
            RemainingTimeInSeconds = 60;
            ElapsedTimeInSeconds = 0;
            Cards = new List<Card>();
        }
    }
}