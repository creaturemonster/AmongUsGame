using System;
using System.Diagnostics.CodeAnalysis;

namespace CrowdPleaser
{
    [ExcludeFromCodeCoverage]
    public class SystemRandom : IRandom
    {
        private Random _rnd = new Random();
        public int Next(int minValue, int maxValue) => _rnd.Next(minValue, maxValue);
        public double NextDouble() => _rnd.NextDouble();
    }
}
