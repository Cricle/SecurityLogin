namespace SecurityLogin.AppLogin
{
    public interface IRandomProvider
    {
        int GetRandom(int min, int max);
    }
}
