using System;
using System.Diagnostics;
using System.Threading;

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
            int width = 40;
            int height = 20;

            int playerX = width / 2;
            int playerY = height / 2;

            double spotlightX = width / 2;
            double spotlightY = height / 2;

            double targetSpotlightX = spotlightX;
            double targetSpotlightY = spotlightY;

            int lives = 5;
            double timeInSpotlight = 0;
            double timeOutOfSpotlight = 0;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Random rnd = new Random();

            double lastFrameTime = sw.Elapsed.TotalSeconds;
            double spotlightChangeTimer = 0;

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
                    if (key == ConsoleKey.UpArrow && playerY > 0) playerY--;
                    if (key == ConsoleKey.DownArrow && playerY < height - 1) playerY++;
                    if (key == ConsoleKey.LeftArrow && playerX > 0) playerX--;
                    if (key == ConsoleKey.RightArrow && playerX < width - 1) playerX++;
                }

                // Spotlight Logic
                spotlightChangeTimer -= dt;
                if (spotlightChangeTimer <= 0)
                {
                    targetSpotlightX = rnd.Next(2, width - 2);
                    targetSpotlightY = rnd.Next(2, height - 2);
                    spotlightChangeTimer = rnd.NextDouble() * 1.5 + 0.5; // Change target every 0.5 to 2.0 seconds
                }

                // Move spotlight towards target
                double dx = targetSpotlightX - spotlightX;
                double dy = targetSpotlightY - spotlightY;
                double dist = Math.Sqrt(dx * dx + dy * dy);
                if (dist > 0.5)
                {
                    double speed = 10.0; // Spotlight speed
                    spotlightX += (dx / dist) * speed * dt;
                    spotlightY += (dy / dist) * speed * dt;
                }

                // Check if player is in spotlight
                // Spotlight is a 3x3 area centered at (spotlightX, spotlightY)
                int sX = (int)Math.Round(spotlightX);
                int sY = (int)Math.Round(spotlightY);

                bool inSpotlight = Math.Abs(playerX - sX) <= 1 && Math.Abs(playerY - sY) <= 1;

                if (inSpotlight)
                {
                    timeInSpotlight += dt;
                    timeOutOfSpotlight = 0; // reset out of spotlight time
                }
                else
                {
                    timeOutOfSpotlight += dt;
                    if (timeOutOfSpotlight >= 3.0)
                    {
                        lives--;
                        timeOutOfSpotlight = 0; // reset so another doesn't leave immediately
                    }
                }

                // Draw
                // We use SetCursorPosition to avoid clear flicker
                Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine($"Audience: {new string('♥', lives)}{new string(' ', 5 - lives)}   ");
                Console.WriteLine($"Time in Spotlight: {timeInSpotlight:F1} / 30.0 s   ");
                Console.WriteLine($"Time Out (loss at 3s): {timeOutOfSpotlight:F1} s   ");
                Console.WriteLine(new string('-', width));

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        bool isSpotlight = Math.Abs(x - sX) <= 1 && Math.Abs(y - sY) <= 1;
                        bool isPlayer = (x == playerX && y == playerY);

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
                Console.WriteLine(new string('-', width));

                // Check Win/Loss
                if (lives <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("GAME OVER! All audience members left.        ");
                    Thread.Sleep(2000);
                    break;
                }

                if (timeInSpotlight >= 30.0)
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
