using System;

namespace CrowdPleaser
{
    public class Game
    {
        private readonly IConsole _console;
        private readonly IRandom _random;
        private readonly IGameLoop _loop;

        public const int InitialLives = 5;
        public const double WinTimeRequired = 30.0;
        public const double TimeOutLossThreshold = 3.0;

        public int Width { get; } = 40;
        public int Height { get; } = 20;

        public int PlayerX { get; private set; }
        public int PlayerY { get; private set; }

        public double SpotlightX { get; private set; }
        public double SpotlightY { get; private set; }
        public double TargetSpotlightX { get; private set; }
        public double TargetSpotlightY { get; private set; }

        public int Lives { get; private set; } = InitialLives;
        public double TimeInSpotlight { get; private set; } = 0;
        public double TimeOutOfSpotlight { get; private set; } = 0;
        public double SpotlightChangeTimer { get; private set; } = 0;

        public bool IsGameOver => Lives <= 0;
        public bool IsWin => TimeInSpotlight >= WinTimeRequired;

        public Game(IConsole console, IRandom random, IGameLoop loop)
        {
            _console = console;
            _random = random;
            _loop = loop;

            PlayerX = Width / 2;
            PlayerY = Height / 2;
            SpotlightX = Width / 2;
            SpotlightY = Height / 2;
            TargetSpotlightX = SpotlightX;
            TargetSpotlightY = SpotlightY;
        }

        public void Play()
        {
            _console.Clear();
            _loop.Run(Update);
        }

        public bool Update(double dt)
        {
            HandleInput();
            UpdateSpotlight(dt);
            CheckSpotlight(dt);
            Draw();

            if (IsGameOver)
            {
                _console.ForegroundColor = ConsoleColor.Red;
                _console.WriteLine("GAME OVER! All audience members left.        ");
                _loop.Sleep(2000);
                return false;
            }

            if (IsWin)
            {
                _console.ForegroundColor = ConsoleColor.Green;
                _console.WriteLine("YOU WIN! You kept the crowd pleased!         ");
                _loop.Sleep(2000);
                return false;
            }

            return true;
        }

        public void HandleInput()
        {
            while (_console.KeyAvailable)
            {
                var key = _console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow && PlayerY > 0) PlayerY--;
                if (key == ConsoleKey.DownArrow && PlayerY < Height - 1) PlayerY++;
                if (key == ConsoleKey.LeftArrow && PlayerX > 0) PlayerX--;
                if (key == ConsoleKey.RightArrow && PlayerX < Width - 1) PlayerX++;
            }
        }

        public void UpdateSpotlight(double dt)
        {
            SpotlightChangeTimer -= dt;
            if (SpotlightChangeTimer <= 0)
            {
                TargetSpotlightX = _random.Next(2, Width - 2);
                TargetSpotlightY = _random.Next(2, Height - 2);
                SpotlightChangeTimer = _random.NextDouble() * 1.5 + 0.5; // Change target every 0.5 to 2.0 seconds
            }

            double dx = TargetSpotlightX - SpotlightX;
            double dy = TargetSpotlightY - SpotlightY;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            if (dist > 0.5)
            {
                double speed = 10.0;
                SpotlightX += (dx / dist) * speed * dt;
                SpotlightY += (dy / dist) * speed * dt;
            }
        }

        public void CheckSpotlight(double dt)
        {
            int sX = (int)Math.Round(SpotlightX);
            int sY = (int)Math.Round(SpotlightY);

            bool inSpotlight = Math.Abs(PlayerX - sX) <= 1 && Math.Abs(PlayerY - sY) <= 1;

            if (inSpotlight)
            {
                TimeInSpotlight += dt;
                TimeOutOfSpotlight = 0;
            }
            else
            {
                TimeOutOfSpotlight += dt;
                if (TimeOutOfSpotlight >= TimeOutLossThreshold)
                {
                    Lives--;
                    TimeOutOfSpotlight = 0;
                }
            }
        }

        public void Draw()
        {
            _console.SetCursorPosition(0, 0);
            _console.ForegroundColor = ConsoleColor.White;
            _console.BackgroundColor = ConsoleColor.Black;
            int displayLives = Math.Max(0, Lives);
            _console.WriteLine($"Audience: {new string('♥', displayLives)}{new string(' ', InitialLives - displayLives)}   ");
            _console.WriteLine($"Time in Spotlight: {TimeInSpotlight:F1} / {WinTimeRequired:F1} s   ");
            _console.WriteLine($"Time Out (loss at {TimeOutLossThreshold:F0}s): {TimeOutOfSpotlight:F1} s   ");
            _console.WriteLine(new string('-', Width));

            int sX = (int)Math.Round(SpotlightX);
            int sY = (int)Math.Round(SpotlightY);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    bool isSpotlight = Math.Abs(x - sX) <= 1 && Math.Abs(y - sY) <= 1;
                    bool isPlayer = (x == PlayerX && y == PlayerY);

                    if (isSpotlight)
                    {
                        _console.BackgroundColor = ConsoleColor.DarkYellow;
                    }
                    else
                    {
                        _console.BackgroundColor = ConsoleColor.Black;
                    }

                    if (isPlayer)
                    {
                        _console.ForegroundColor = ConsoleColor.Green;
                        _console.Write("P");
                    }
                    else
                    {
                        _console.Write(" ");
                    }
                }
                _console.BackgroundColor = ConsoleColor.Black;
                _console.WriteLine();
            }
            _console.WriteLine(new string('-', Width));
        }
    }
}
