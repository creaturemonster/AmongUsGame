using System;

namespace CrowdPleaser.Logic
{
    public enum GameState
    {
        Playing,
        Win,
        Loss
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class SpotlightGameLogic
    {
        public int Width { get; }
        public int Height { get; }

        public int PlayerX { get; private set; }
        public int PlayerY { get; private set; }

        public double SpotlightX { get; private set; }
        public double SpotlightY { get; private set; }

        private double _targetSpotlightX;
        private double _targetSpotlightY;

        public int Lives { get; private set; }
        public double TimeInSpotlight { get; private set; }
        public double TimeOutOfSpotlight { get; private set; }

        public GameState CurrentState { get; private set; }

        private Random _rnd;
        private double _spotlightChangeTimer;

        public SpotlightGameLogic(int width = 40, int height = 20)
        {
            Width = width;
            Height = height;

            PlayerX = width / 2;
            PlayerY = height / 2;

            SpotlightX = width / 2;
            SpotlightY = height / 2;

            _targetSpotlightX = SpotlightX;
            _targetSpotlightY = SpotlightY;

            Lives = 5;
            TimeInSpotlight = 0;
            TimeOutOfSpotlight = 0;

            CurrentState = GameState.Playing;

            _rnd = new Random();
            _spotlightChangeTimer = 0;
        }

        public void MovePlayer(Direction dir)
        {
            if (CurrentState != GameState.Playing) return;

            switch (dir)
            {
                case Direction.Up:
                    if (PlayerY > 0) PlayerY--;
                    break;
                case Direction.Down:
                    if (PlayerY < Height - 1) PlayerY++;
                    break;
                case Direction.Left:
                    if (PlayerX > 0) PlayerX--;
                    break;
                case Direction.Right:
                    if (PlayerX < Width - 1) PlayerX++;
                    break;
            }
        }

        public void Update(double dt)
        {
            if (CurrentState != GameState.Playing) return;

            // Spotlight Logic
            _spotlightChangeTimer -= dt;
            if (_spotlightChangeTimer <= 0)
            {
                _targetSpotlightX = _rnd.Next(2, Width - 2);
                _targetSpotlightY = _rnd.Next(2, Height - 2);
                _spotlightChangeTimer = _rnd.NextDouble() * 1.5 + 0.5; // Change target every 0.5 to 2.0 seconds
            }

            // Move spotlight towards target
            double dx = _targetSpotlightX - SpotlightX;
            double dy = _targetSpotlightY - SpotlightY;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            if (dist > 0.5)
            {
                double speed = 10.0; // Spotlight speed
                SpotlightX += (dx / dist) * speed * dt;
                SpotlightY += (dy / dist) * speed * dt;
            }

            // Check if player is in spotlight
            // Spotlight is a 3x3 area centered at (spotlightX, spotlightY)
            int sX = (int)Math.Round(SpotlightX);
            int sY = (int)Math.Round(SpotlightY);

            bool inSpotlight = Math.Abs(PlayerX - sX) <= 1 && Math.Abs(PlayerY - sY) <= 1;

            if (inSpotlight)
            {
                TimeInSpotlight += dt;
                TimeOutOfSpotlight = 0; // reset out of spotlight time
            }
            else
            {
                TimeOutOfSpotlight += dt;
                if (TimeOutOfSpotlight >= 3.0)
                {
                    Lives--;
                    TimeOutOfSpotlight = 0; // reset so another doesn't leave immediately
                }
            }

            // Check Win/Loss
            if (Lives <= 0)
            {
                CurrentState = GameState.Loss;
            }
            else if (TimeInSpotlight >= 30.0)
            {
                CurrentState = GameState.Win;
            }
        }
    }
}
