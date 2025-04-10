using System;
using System.Windows.Threading;

namespace MemoryGame.Services
{
    public class GameTimerService
    {
        private DispatcherTimer _gameTimer;
        private int _gameTimeInSeconds;
        private int _remainingTimeInSeconds;

        public event Action TimerTick;

        public int GameTimeInSeconds
        {
            get => _gameTimeInSeconds;
            set => _gameTimeInSeconds = value;
        }

        public int RemainingTimeInSeconds
        {
            get => _remainingTimeInSeconds;
            private set
            {
                _remainingTimeInSeconds = value;
                TimerTick?.Invoke();
            }
        }

        public GameTimerService()
        {
            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromSeconds(1);
            _gameTimer.Tick += GameTimerTick;
            _gameTimeInSeconds = 60; // Default Time
        }

        private void GameTimerTick(object sender, EventArgs e)
        {
            RemainingTimeInSeconds--;
        }

        public void Start()
        {
            RemainingTimeInSeconds = GameTimeInSeconds;
            _gameTimer.Start();
        }

        public void Stop()
        {
            _gameTimer.Stop();
        }

        public bool IsRunning() => _gameTimer.IsEnabled;

        public string GetTimeDisplay()
        {
            TimeSpan time = TimeSpan.FromSeconds(RemainingTimeInSeconds);
            return $"{time.Minutes:D2}:{time.Seconds:D2}";
        }
    }
}