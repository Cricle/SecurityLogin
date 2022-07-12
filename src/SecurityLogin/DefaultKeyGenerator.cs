using System;
using System.Threading.Tasks;

namespace SecurityLogin.AppLogin
{
    public class DefaultKeyGenerator : IKeyGenerator
    {
        public static readonly DefaultKeyGenerator Default = new DefaultKeyGenerator(DefaultRandomProvider.Instance);

        public DefaultKeyGenerator(IRandomProvider randomProvider)
        {
            RandomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));
        }

        public IRandomProvider RandomProvider { get; }

        public Task<string> GenSecretKeyAsync()
        {
            var appSecretKey = Guid.NewGuid().ToString("N") + DateTime.Now.Ticks + RandomProvider.GetRandom(1111, 9999);
            var res = Md5EncryptionHelper.Instance.ComputeHashToString(appSecretKey);
            return Task.FromResult(res);
        }
    }
}
