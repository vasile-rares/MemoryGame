using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MemoryGame.Models;

namespace MemoryGame.Services
{
    public class GameService
    {
        private Random _random = new Random();
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

        public void StartGame(ObservableCollection<Card> cards)
        {
            _totalPairs = cards.Count / 2;

            _matchedPairs = cards.Count(c => c.IsMatched) / 2;

            _firstSelectedCard = null;
            _secondSelectedCard = null;

            _isGameInProgress = true;
        }

        public void EndGame(bool isWon)
        {
            _isGameInProgress = false;
        }

        public void SelectCard(Card selectedCard, Action<Card> onCardFlipped, Action<bool> onGameEnded)
        {
            if (!_isGameInProgress || selectedCard == null || selectedCard.IsFlipped || selectedCard.IsMatched)
                return;

            selectedCard.IsFlipped = true;

            // Notify about the card flipping
            onCardFlipped?.Invoke(selectedCard);

            if (_firstSelectedCard == null)
            {
                _firstSelectedCard = selectedCard;
                return;
            }

            if (_secondSelectedCard == null && selectedCard != _firstSelectedCard)
            {
                _secondSelectedCard = selectedCard;

                if (_firstSelectedCard.ImagePath == _secondSelectedCard.ImagePath)
                {
                    _firstSelectedCard.IsMatched = true;
                    _secondSelectedCard.IsMatched = true;

                    // Notify about match status change
                    onCardFlipped?.Invoke(_firstSelectedCard);
                    onCardFlipped?.Invoke(_secondSelectedCard);

                    _matchedPairs++;

                    _firstSelectedCard = null;
                    _secondSelectedCard = null;

                    // Check if game is complete
                    if (_matchedPairs >= _totalPairs)
                    {
                        EndGame(true);
                        onGameEnded?.Invoke(true);
                    }
                }
            }
            else if (_secondSelectedCard != null)
            {
                // Third card - flip back the unmatched pair
                if (!_firstSelectedCard.IsMatched)
                {
                    _firstSelectedCard.IsFlipped = false;
                    // Notify about flipping back
                    onCardFlipped?.Invoke(_firstSelectedCard);
                }

                if (!_secondSelectedCard.IsMatched)
                {
                    _secondSelectedCard.IsFlipped = false;
                    // Notify about flipping back
                    onCardFlipped?.Invoke(_secondSelectedCard);
                }

                // Set this card as the first selected card
                _firstSelectedCard = selectedCard;
                _secondSelectedCard = null;
            }
        }
    }
}