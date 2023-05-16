namespace SecurityLogin.Mode.RSA.Helpers
{
    public readonly struct RSAKey
    {
        public readonly string PublicKey;

        public readonly string PrivateKey;

        public RSAKey(string publicKey, string privateKey)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }
    }
}
