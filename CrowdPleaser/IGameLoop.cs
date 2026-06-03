using System;

namespace CrowdPleaser
{
    public interface IGameLoop
    {
        void Run(Func<double, bool> updateCallback);
        void Sleep(int milliseconds);
    }
}
