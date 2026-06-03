using System;
using System.Diagnostics;
using System.Threading;
using CrowdPleaser.Logic;

namespace CrowdPleaser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            while (true)
            {
                PlayGame();

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Press 'Q' to quit, or any other key to play again...");
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Q)
                {
                    break;
                }
            }
        }

        static void PlayGame()
        {
            var game = new SpotlightGameLogic();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            double lastFrameTime = sw.Elapsed.TotalSeconds;

            Console.Clear();

            while (true)
            {
                double currentTime = sw.Elapsed.TotalSeconds;
                double dt = currentTime - lastFrameTime;
                lastFrameTime = currentTime;

                // Handle Input
                while (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.UpArrow) game.MovePlayer(Direction.Up);
                    if (key == ConsoleKey.DownArrow) game.MovePlayer(Direction.Down);
                    if (key == ConsoleKey.LeftArrow) game.MovePlayer(Direction.Left);
                    if (key == ConsoleKey.RightArrow) game.MovePlayer(Direction.Right);
                }

                game.Update(dt);

                // Draw
                // We use SetCursorPosition to avoid clear flicker
                Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine($"Audience: {new string('♥', game.Lives)}{new string(' ', 5 - game.Lives)}   ");
                Console.WriteLine($"Time in Spotlight: {game.TimeInSpotlight:F1} / 30.0 s   ");
                Console.WriteLine($"Time Out (loss at 3s): {game.TimeOutOfSpotlight:F1} s   ");
                Console.WriteLine(new string('-', game.Width));

                int sX = (int)Math.Round(game.SpotlightX);
                int sY = (int)Math.Round(game.SpotlightY);

                for (int y = 0; y < game.Height; y++)
                {
                    for (int x = 0; x < game.Width; x++)
                    {
                        bool isSpotlight = Math.Abs(x - sX) <= 1 && Math.Abs(y - sY) <= 1;
                        bool isPlayer = (x == game.PlayerX && y == game.PlayerY);

                        if (isSpotlight)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkYellow;
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.Black;
                        }

                        if (isPlayer)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("P");
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine();
                }
                Console.WriteLine(new string('-', game.Width));

                // Check Win/Loss
                if (game.CurrentState == GameState.Loss)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("GAME OVER! All audience members left.        ");
                    Thread.Sleep(2000);
                    break;
                }

                if (game.CurrentState == GameState.Win)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("YOU WIN! You kept the crowd pleased!         ");
                    Thread.Sleep(2000);
                    break;
                }

                Thread.Sleep(33); // roughly 30 FPS
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
