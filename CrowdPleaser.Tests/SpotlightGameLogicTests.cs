using Xunit;
using CrowdPleaser.Logic;

namespace CrowdPleaser.Tests
{
    public class SpotlightGameLogicTests
    {
        [Fact]
        public void InitialState_IsCorrect()
        {
            var game = new SpotlightGameLogic(40, 20);

            Assert.Equal(40, game.Width);
            Assert.Equal(20, game.Height);
            Assert.Equal(20, game.PlayerX);
            Assert.Equal(10, game.PlayerY);
            Assert.Equal(5, game.Lives);
            Assert.Equal(0, game.TimeInSpotlight);
            Assert.Equal(0, game.TimeOutOfSpotlight);
            Assert.Equal(GameState.Playing, game.CurrentState);
        }

        [Fact]
        public void MovePlayer_UpdatesPosition_WhenWithinBounds()
        {
            var game = new SpotlightGameLogic(40, 20);

            int initialX = game.PlayerX;
            int initialY = game.PlayerY;

            game.MovePlayer(Direction.Up);
            Assert.Equal(initialY - 1, game.PlayerY);

            game.MovePlayer(Direction.Down);
            Assert.Equal(initialY, game.PlayerY);

            game.MovePlayer(Direction.Left);
            Assert.Equal(initialX - 1, game.PlayerX);

            game.MovePlayer(Direction.Right);
            Assert.Equal(initialX, game.PlayerX);
        }

        [Fact]
        public void MovePlayer_DoesNotMoveOutsideBounds()
        {
            var game = new SpotlightGameLogic(40, 20);

            // Move player to top left
            for (int i = 0; i < 40; i++) game.MovePlayer(Direction.Left);
            for (int i = 0; i < 20; i++) game.MovePlayer(Direction.Up);

            Assert.Equal(0, game.PlayerX);
            Assert.Equal(0, game.PlayerY);

            // Attempt to move out of bounds
            game.MovePlayer(Direction.Left);
            game.MovePlayer(Direction.Up);

            Assert.Equal(0, game.PlayerX);
            Assert.Equal(0, game.PlayerY);
        }

        [Fact]
        public void TimeInSpotlight_Accumulates_WhenPlayerIsInSpotlight()
        {
            var game = new SpotlightGameLogic(40, 20);
            // Initially player and spotlight are both at the center (20, 10)

            // Use a small dt so the spotlight doesn't move out of range
            game.Update(0.01);

            Assert.True(game.TimeInSpotlight >= 0.01);
            Assert.Equal(0, game.TimeOutOfSpotlight);
        }

        [Fact]
        public void TimeOutOfSpotlight_Accumulates_WhenPlayerLeavesSpotlight()
        {
            var game = new SpotlightGameLogic(40, 20);

            // Move player far away
            for (int i = 0; i < 20; i++) game.MovePlayer(Direction.Left);

            game.Update(1.0);

            Assert.Equal(0, game.TimeInSpotlight);
            Assert.Equal(1.0, game.TimeOutOfSpotlight);
        }

        [Fact]
        public void PlayerLosesLife_WhenOutOfSpotlightFor3Seconds()
        {
            var game = new SpotlightGameLogic(40, 20);

            // Move player far away
            for (int i = 0; i < 20; i++) game.MovePlayer(Direction.Left);

            game.Update(3.0);

            Assert.Equal(4, game.Lives);
            Assert.Equal(0, game.TimeOutOfSpotlight); // It resets
        }

        [Fact]
        public void GameLossState_Triggers_WhenLivesReachZero()
        {
            var game = new SpotlightGameLogic(40, 20);

            // Move player far away
            for (int i = 0; i < 20; i++) game.MovePlayer(Direction.Left);

            // Lose 5 lives
            for (int i = 0; i < 5; i++)
            {
                game.Update(3.0);
            }

            Assert.Equal(0, game.Lives);
            Assert.Equal(GameState.Loss, game.CurrentState);
        }

        [Fact]
        public void GameWinState_Triggers_WhenTimeInSpotlightReaches30()
        {
            var game = new SpotlightGameLogic(40, 20);

            // Update in small increments and move player towards spotlight
            // simulating a perfect player so they win
            for (int i = 0; i < 3000; i++)
            {
                game.Update(0.01);

                int targetX = (int)Math.Round(game.SpotlightX);
                int targetY = (int)Math.Round(game.SpotlightY);

                if (game.PlayerX < targetX) game.MovePlayer(Direction.Right);
                if (game.PlayerX > targetX) game.MovePlayer(Direction.Left);
                if (game.PlayerY < targetY) game.MovePlayer(Direction.Down);
                if (game.PlayerY > targetY) game.MovePlayer(Direction.Up);
            }

            Assert.Equal(GameState.Win, game.CurrentState);
        }
    }
}
