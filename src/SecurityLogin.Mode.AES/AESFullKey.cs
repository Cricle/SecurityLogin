namespace SecurityLogin.Mode.AES
{
    public class AESFullKey : IIdentityable
    {
        public string Identity { get; set; }

        public string IV { get; set; }

        public string Key { get; set; }
    }
}
