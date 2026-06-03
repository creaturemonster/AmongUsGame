using System;
using System.Diagnostics.CodeAnalysis;

namespace CrowdPleaser
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        static void Main(string[] args)
        {
            IConsole console = new SystemConsole();
            IRandom random = new SystemRandom();
            IGameLoop loop = new RealGameLoop();

            console.CursorVisible = false;
            while (true)
            {
                Game game = new Game(console, random, loop);
                game.Play();

                console.BackgroundColor = ConsoleColor.Black;
                console.Clear();
                console.ForegroundColor = ConsoleColor.White;
                console.WriteLine("Press 'Q' to quit, or any other key to play again...");
                var key = console.ReadKey(true).Key;
                if (key == ConsoleKey.Q)
                {
                    break;
                }
            }
        }
    }
}
