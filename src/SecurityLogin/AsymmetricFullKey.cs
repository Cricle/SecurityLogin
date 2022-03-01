namespace SecurityLogin
{
    public class AsymmetricFullKey: IIdentityable
    {
        public string Identity { get; set; }

        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }
    }
}
