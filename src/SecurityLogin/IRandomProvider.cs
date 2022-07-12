namespace SecurityLogin
{
    public interface IRandomProvider
    {
        int GetRandom(int min, int max);
    }
}
