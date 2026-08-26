using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace CrowdPleaser
{
    [ExcludeFromCodeCoverage]
    public class SystemRandom : IRandom
    {
        public int Next(int minValue, int maxValue) => RandomNumberGenerator.GetInt32(minValue, maxValue);
        public double NextDouble()
        {
            byte[] bytes = new byte[8];
            RandomNumberGenerator.Fill(bytes);
            ulong ul = BitConverter.ToUInt64(bytes, 0);
            return (ul >> 11) * (1.0 / (1ul << 53));
        }
    }
}
