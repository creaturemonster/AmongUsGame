using System;
using System.Diagnostics;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace CrowdPleaser
{
    [ExcludeFromCodeCoverage]
    public class RealGameLoop : IGameLoop
    {
        public void Run(Func<double, bool> updateCallback)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            double lastFrameTime = sw.Elapsed.TotalSeconds;

            while (true)
            {
                double currentTime = sw.Elapsed.TotalSeconds;
                double dt = currentTime - lastFrameTime;
                lastFrameTime = currentTime;

                bool continueRunning = updateCallback(dt);
                if (!continueRunning) break;

                Thread.Sleep(33);
            }
        }

        public void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }
    }
}
