namespace Uqs.Weather.Providers
{
    public class RandomProvider : IRandomProvider
    {
        public int Next(int min, int max )
        {
            return Random.Shared.Next(min, max);
        }
    }
}
