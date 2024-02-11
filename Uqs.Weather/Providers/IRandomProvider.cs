namespace Uqs.Weather.Providers
{
    public interface IRandomProvider
    {
         int Next(int min, int max);
    }
}
