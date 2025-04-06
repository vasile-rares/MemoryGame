using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MemoryGame.Models;

namespace MemoryGame.Services
{
    public class GameService
    {
        private Random _random = new Random();

        public event EventHandler<bool> GameEnded;

        public event EventHandler<Card[]> CardsMatched;

        public event EventHandler<Card[]> CardsMismatched;

        private Card _firstSelectedCard;
        private Card _secondSelectedCard;
        private int _matchedPairs = 0;
        private int _totalPairs = 0;
        private bool _isGameInProgress = false;

        public bool IsGameInProgress => _isGameInProgress;
        public int TotalPairs => _totalPairs;
        public int MatchedPairs => _matchedPairs;

        public ObservableCollection<Card> CreateGameBoard(int rows, int columns, int categoryId)
        {
            ObservableCollection<Card> cards = new ObservableCollection<Card>();

            int totalCards = rows * columns;
            _totalPairs = totalCards / 2;
            _matchedPairs = 0;

            // Generate cards
            List<Card> newCards = new List<Card>();
            for (int i = 1; i <= _totalPairs; i++)
            {
                // Create two cards with the same image (a pair)
                string imagePath = $"pack://application:,,,/Images/Category{categoryId}/card{i}.jpg";

                newCards.Add(new Card
                {
                    Id = i * 2 - 1,
                    ImagePath = imagePath,
                    IsFlipped = false,
                    IsMatched = false
                });

                newCards.Add(new Card
                {
                    Id = i * 2,
                    ImagePath = imagePath,
                    IsFlipped = false,
                    IsMatched = false
                });
            }

            // Shuffle cards (Fisher-Yates algorithm)
            for (int i = newCards.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                Card temp = newCards[i];
                newCards[i] = newCards[j];
                newCards[j] = temp;
            }

            // Add to observable collection
            foreach (var card in newCards)
            {
                cards.Add(card);
            }

            return cards;
        }

        public void StartGame()
        {
            _isGameInProgress = true;
            _matchedPairs = 0;
            _firstSelectedCard = null;
            _secondSelectedCard = null;
        }

        public void EndGame(bool isWon)
        {
            _isGameInProgress = false;
            GameEnded?.Invoke(this, isWon);
        }

        public void SelectCard(Card selectedCard)
        {
            if (!_isGameInProgress || selectedCard == null || selectedCard.IsFlipped || selectedCard.IsMatched)
                return;

            // Flip the card
            selectedCard.IsFlipped = true;

            // Check if this is the first card selected
            if (_firstSelectedCard == null)
            {
                _firstSelectedCard = selectedCard;
                return;
            }

            // Check if this is the second card selected
            if (_secondSelectedCard == null && selectedCard != _firstSelectedCard)
            {
                _secondSelectedCard = selectedCard;

                // Check if the cards match
                if (_firstSelectedCard.ImagePath == _secondSelectedCard.ImagePath)
                {
                    // Cards match
                    _firstSelectedCard.IsMatched = true;
                    _secondSelectedCard.IsMatched = true;
                    _matchedPairs++;

                    Card[] matchedCards = new Card[] { _firstSelectedCard, _secondSelectedCard };
                    CardsMatched?.Invoke(this, matchedCards);

                    // Reset selected cards
                    _firstSelectedCard = null;
                    _secondSelectedCard = null;

                    // Check if game is complete
                    if (_matchedPairs >= _totalPairs)
                    {
                        EndGame(true);
                    }
                }
                else
                {
                    // Cards don't match
                    Card[] mismatchedCards = new Card[] { _firstSelectedCard, _secondSelectedCard };
                    CardsMismatched?.Invoke(this, mismatchedCards);
                }
            }
            else if (_secondSelectedCard != null)
            {
                // Third card - flip back the unmatched pair
                if (!_firstSelectedCard.IsMatched)
                {
                    _firstSelectedCard.IsFlipped = false;
                }

                if (!_secondSelectedCard.IsMatched)
                {
                    _secondSelectedCard.IsFlipped = false;
                }

                // Set this card as the first selected card
                _firstSelectedCard = selectedCard;
                _secondSelectedCard = null;
            }
        }
    }
}