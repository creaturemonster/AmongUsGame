using System;
using System.Collections.Generic;
using Xunit;
using CrowdPleaser;

namespace CrowdPleaser.Tests
{
    public class MockConsole : IConsole
    {
        public bool CursorVisible { get; set; }
        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }

        public Queue<ConsoleKeyInfo> Keys = new Queue<ConsoleKeyInfo>();
        public bool KeyAvailable => Keys.Count > 0;

        public ConsoleKeyInfo ReadKey(bool intercept) => Keys.Dequeue();

        public List<string> WrittenLines = new List<string>();
        public List<string> WrittenStrings = new List<string>();

        public void Clear() {}
        public void SetCursorPosition(int left, int top) {}
        public void WriteLine(string value) => WrittenLines.Add(value);
        public void WriteLine() => WrittenLines.Add("");
        public void Write(string value) => WrittenStrings.Add(value);
    }

    public class MockRandom : IRandom
    {
        public int NextValue { get; set; } = 20; // 20 is X center, 10 is Y center
        public double NextDoubleValue { get; set; } = 1.0;

        private int _callCount = 0;
        public int Next(int minValue, int maxValue)
        {
            _callCount++;
            return _callCount % 2 != 0 ? 20 : 10;
        }
        public double NextDouble() => NextDoubleValue;
    }

    public class MockGameLoop : IGameLoop
    {
        public double[] DtValues = { 0.1 };
        public int Iterations { get; set; } = 1;
        public int IterationCount = 0;

        public void Run(Func<double, bool> updateCallback)
        {
            for (int i = 0; i < Iterations; i++)
            {
                IterationCount++;
                double dt = DtValues[i % DtValues.Length];
                if (!updateCallback(dt)) break;
            }
        }

        public void Sleep(int milliseconds) {}
    }

    public class GameTests
    {
        [Fact]
        public void Game_Initialization_SetsDefaults()
        {
            var console = new MockConsole();
            var random = new MockRandom();
            var loop = new MockGameLoop();
            var game = new Game(console, random, loop);

            Assert.Equal(40, game.Width);
            Assert.Equal(20, game.Height);
            Assert.Equal(20, game.PlayerX);
            Assert.Equal(10, game.PlayerY);
            Assert.Equal(20, game.SpotlightX);
            Assert.Equal(10, game.SpotlightY);
            Assert.Equal(5, game.Lives);
            Assert.Equal(0, game.TimeInSpotlight);
            Assert.Equal(0, game.TimeOutOfSpotlight);
        }

        [Fact]
        public void HandleInput_MovesPlayer()
        {
            var console = new MockConsole();
            var game = new Game(console, new MockRandom(), new MockGameLoop());

            // Initial pos: 20, 10
            console.Keys.Enqueue(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false));
            console.Keys.Enqueue(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false));
            game.HandleInput();

            Assert.Equal(19, game.PlayerX);
            Assert.Equal(9, game.PlayerY);

            console.Keys.Enqueue(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));
            console.Keys.Enqueue(new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false));
            game.HandleInput();

            Assert.Equal(20, game.PlayerX);
            Assert.Equal(10, game.PlayerY);
        }

        [Fact]
        public void HandleInput_RespectsBounds()
        {
            var console = new MockConsole();
            var game = new Game(console, new MockRandom(), new MockGameLoop());

            for (int i = 0; i < 50; i++)
                console.Keys.Enqueue(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false));
            for (int i = 0; i < 50; i++)
                console.Keys.Enqueue(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false));
            game.HandleInput();

            Assert.Equal(0, game.PlayerX);
            Assert.Equal(0, game.PlayerY);

            for (int i = 0; i < 50; i++)
                console.Keys.Enqueue(new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false));
            for (int i = 0; i < 50; i++)
                console.Keys.Enqueue(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));
            game.HandleInput();

            Assert.Equal(39, game.PlayerX);
            Assert.Equal(19, game.PlayerY);
        }

        [Fact]
        public void UpdateSpotlight_ChangesTargetAndMoves()
        {
            var random = new MockRandom { NextValue = 5, NextDoubleValue = 0.5 }; // target changes every 0.5*1.5+0.5 = 1.25s
            var game = new Game(new MockConsole(), random, new MockGameLoop());

            Assert.Equal(20, game.TargetSpotlightX);
            Assert.Equal(10, game.TargetSpotlightY);

            game.UpdateSpotlight(0.1); // Timer drops below 0
            Assert.Equal(20, game.TargetSpotlightX);
            Assert.Equal(10, game.TargetSpotlightY); // Target hasn't really changed position

            double prevX = game.SpotlightX;
            double prevY = game.SpotlightY;

            // Give it a real target
            game.GetType().GetProperty("TargetSpotlightX").SetValue(game, 0);

            game.UpdateSpotlight(0.1);

            Assert.NotEqual(prevX, game.SpotlightX);
        }

        [Fact]
        public void CheckSpotlight_InSpotlight_IncreasesTime()
        {
            var game = new Game(new MockConsole(), new MockRandom(), new MockGameLoop());

            game.CheckSpotlight(0.1);

            Assert.Equal(0.1, game.TimeInSpotlight);
            Assert.Equal(0, game.TimeOutOfSpotlight);
        }

        [Fact]
        public void CheckSpotlight_OutSpotlight_DecreasesLives()
        {
            var console = new MockConsole();
            var game = new Game(console, new MockRandom(), new MockGameLoop());

            // Move player out of spotlight (spotlight is at 20, 10)
            for (int i = 0; i < 5; i++)
                console.Keys.Enqueue(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false));
            game.HandleInput();

            game.CheckSpotlight(1.0);
            Assert.Equal(5, game.Lives);
            Assert.Equal(1.0, game.TimeOutOfSpotlight);

            game.CheckSpotlight(2.0); // Total 3.0s out
            Assert.Equal(4, game.Lives);
            Assert.Equal(0.0, game.TimeOutOfSpotlight); // Reset
        }

        [Fact]
        public void Update_WinCondition()
        {
            var console = new MockConsole();
            var loop = new MockGameLoop { Iterations = 1 };

            var random = new MockRandom { NextValue = 20, NextDoubleValue = 1.0 };

            var game = new Game(console, random, loop);

            // Update 300 times at dt 0.1
            for (int i = 0; i < 300; i++)
            {
                game.Update(0.1);
            }

            Assert.True(game.IsWin);
            Assert.Equal(ConsoleColor.Green, console.ForegroundColor);
        }

        [Fact]
        public void Update_LossCondition()
        {
            var console = new MockConsole();
            var loop = new MockGameLoop { Iterations = 1 };
            var game = new Game(console, new MockRandom(), loop);

            // Move out of spotlight
            for (int i = 0; i < 5; i++)
                console.Keys.Enqueue(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false));
            game.HandleInput();

            // Lose all lives
            game.CheckSpotlight(3.0); // 4 lives
            game.CheckSpotlight(3.0); // 3 lives
            game.CheckSpotlight(3.0); // 2 lives
            game.CheckSpotlight(3.0); // 1 life
            game.CheckSpotlight(3.0); // 0 lives

            // Advance should trigger loss and return false
            bool result = game.Update(0.1);

            Assert.False(result);
            Assert.True(game.IsGameOver);
            Assert.Equal(ConsoleColor.Red, console.ForegroundColor);
        }

        [Fact]
        public void Play_StartsLoop()
        {
            var console = new MockConsole();
            var loop = new MockGameLoop { Iterations = 5 };
            var game = new Game(console, new MockRandom(), loop);

            game.Play();

            Assert.Equal(5, loop.IterationCount);
        }

        [Fact]
        public void Draw_OutputsCorrectUI()
        {
            var console = new MockConsole();
            var game = new Game(console, new MockRandom(), new MockGameLoop());

            // Add some time in spotlight to test formatting
            game.CheckSpotlight(1.5);

            game.Draw();

            // First few lines should be status
            Assert.Contains(console.WrittenLines, l => l.Contains("Audience: ♥♥♥♥♥"));
            Assert.Contains(console.WrittenLines, l => l.Contains("Time in Spotlight: 1.5 / 30.0 s"));
            Assert.Contains(console.WrittenLines, l => l.Contains("Time Out (loss at 3s): 0.0 s"));

            // Contains player marker
            Assert.Contains(console.WrittenStrings, s => s == "P");
        }

        [Fact]
        public void Draw_WithNegativeLives_DisplaysZeroHearts()
        {
            var console = new MockConsole();
            var game = new Game(console, new MockRandom(), new MockGameLoop());

            // Move out of spotlight
            for (int i = 0; i < 5; i++)
                console.Keys.Enqueue(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false));
            game.HandleInput();

            // Lose all 5 lives and go to -1
            for (int i = 0; i < 6; i++)
                game.CheckSpotlight(3.0);

            Assert.Equal(-1, game.Lives);

            game.Draw();

            // Should display 0 hearts, 5 spaces
            Assert.Contains(console.WrittenLines, l => l.Contains("Audience:      "));
            Assert.DoesNotContain(console.WrittenLines, l => l.Contains("♥"));
        }
    }
}
