namespace CrowdPleaser
{
    public interface IRandom
    {
        int Next(int minValue, int maxValue);
        double NextDouble();
    }
}
