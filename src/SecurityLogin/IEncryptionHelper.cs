namespace SecurityLogin
{
    public interface IEncryptionHelper
    {
        byte[] ComputeHash(byte[] input);

        byte[] ComputeHash(string input);

        string ComputeHashToString(byte[] input);

        string ComputeHashToString(string input);
    }
}
